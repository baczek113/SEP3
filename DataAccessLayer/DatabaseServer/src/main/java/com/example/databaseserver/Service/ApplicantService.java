package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.Applicant;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.UserRole;
import com.example.databaseserver.Repositories.ApplicantRepository;
import com.example.databaseserver.Repositories.LocationRepository;
import com.example.databaseserver.generated.ApplicantServiceGrpc;
import com.example.databaseserver.generated.CreateApplicantRequest;
import com.example.databaseserver.generated.ApplicantResponse;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;

@GrpcService
public class ApplicantService extends ApplicantServiceGrpc.ApplicantServiceImplBase {

    private final ApplicantRepository applicantRepository;
    private final LocationRepository locationRepository;

    @Autowired
    public ApplicantService(ApplicantRepository applicantRepository,
                            LocationRepository locationRepository) {
        this.applicantRepository = applicantRepository;
        this.locationRepository = locationRepository;
    }

    @Override
    @Transactional
    public void createApplicant(CreateApplicantRequest request,
                                StreamObserver<ApplicantResponse> responseObserver) {
        try {
            User user = new User(
                    request.getEmail(),
                    request.getPasswordHash(),
                    UserRole.applicant,
                    request.getName()
            );

            Location location = new Location(
                    request.getCity(),
                    request.getPostcode(),
                    request.getAddress()
            );
            Location savedLocation = locationRepository.save(location);

            Applicant applicant = new Applicant(
                    user,
                    request.getExperience(),
                    savedLocation
            );

            Applicant savedApplicant = applicantRepository.save(applicant);

            ApplicantResponse response = ApplicantResponse.newBuilder()
                    .setId(savedApplicant.getId())
                    .setName(savedApplicant.getUser().getName())
                    .setEmail(savedApplicant.getUser().getEmail())
                    .setExperience(
                            savedApplicant.getExperience() == null
                                    ? ""
                                    : savedApplicant.getExperience()
                    )
                    .setCity(savedApplicant.getLocation().getCity())
                    .setPostcode(
                            savedApplicant.getLocation().getPostcode() == null
                                    ? ""
                                    : savedApplicant.getLocation().getPostcode()
                    )
                    .setAddress(
                            savedApplicant.getLocation().getAddress() == null
                                    ? ""
                                    : savedApplicant.getLocation().getAddress()
                    )
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }
}

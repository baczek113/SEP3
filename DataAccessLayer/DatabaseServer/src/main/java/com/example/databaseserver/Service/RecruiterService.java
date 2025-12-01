package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.*;
import com.example.databaseserver.Entities.Company;
import com.example.databaseserver.Entities.Recruiter;
import com.example.databaseserver.Repositories.CompanyRepository;
import com.example.databaseserver.Repositories.RecruiterRepository;
import com.example.databaseserver.Repositories.RepresentativeRepository;
import com.example.databaseserver.Repositories.UserRepository;
import com.example.databaseserver.generated.RecruiterServiceGrpc;
import com.example.databaseserver.generated.RegisterRecruiterRequest;
import com.example.databaseserver.generated.RecruiterResponse;
import com.example.databaseserver.generated.GetRecruitersForCompanyRequest;
import com.example.databaseserver.generated.GetRecruitersForCompanyResponse;
import com.example.databaseserver.generated.GetRecruiterByIdRequest;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.List;

@GrpcService
public class RecruiterService extends RecruiterServiceGrpc.RecruiterServiceImplBase {

    private final RecruiterRepository recruiterRepository;
    private final UserRepository userRepository;
    private final RepresentativeRepository representativeRepository;
    private final CompanyRepository companyRepository;

    @Autowired
    public RecruiterService(RecruiterRepository recruiterRepository,
                            UserRepository userRepository,
                            RepresentativeRepository representativeRepository,
                            CompanyRepository companyRepository) {
        this.recruiterRepository = recruiterRepository;
        this.userRepository = userRepository;
        this.representativeRepository = representativeRepository;
        this.companyRepository = companyRepository;
    }

    @Override
    @Transactional
    public void registerRecruiter(RegisterRecruiterRequest request,
                                  StreamObserver<RecruiterResponse> responseObserver) {

        try {

            User user = new User(
                    request.getEmail(),
                    request.getPasswordHash(),
                    UserRole.recruiter,
                    request.getName()
            );
            User savedUser = userRepository.save(user);

            CompanyRepresentative hiredBy = representativeRepository
                    .findById(request.getHiredByRepresentativeId())
                    .orElse(null);

            Company worksIn = companyRepository
                    .findById(request.getWorksInCompanyId())
                    .orElse(null);

            Recruiter recruiter = new Recruiter(
                    savedUser,
                    request.getPosition(),
                    hiredBy,
                    worksIn
            );
            Recruiter savedRecruiter = recruiterRepository.save(recruiter);

            RecruiterResponse response = RecruiterResponse.newBuilder()
                    .setId(savedRecruiter.getId())
                    .setEmail(safe(savedUser.getEmail()))
                    .setName(safe(savedUser.getName()))
                    .setPosition(safe(savedRecruiter.getPosition()))
                    .setHiredById(
                            savedRecruiter.getHiredBy() != null
                                    ? savedRecruiter.getHiredBy().getId()
                                    : 0L
                    )
                    .setWorksInCompanyId(
                            savedRecruiter.getWorksIn() != null
                                    ? savedRecruiter.getWorksIn().getId()
                                    : 0L
                    )
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void getRecruitersForCompany(
            GetRecruitersForCompanyRequest request,
            StreamObserver<GetRecruitersForCompanyResponse> responseObserver) {

        try {
            long companyId = request.getCompanyId();

            List<Recruiter> recruiters =
                    recruiterRepository.findByWorksIn_Id(companyId);

            GetRecruitersForCompanyResponse.Builder responseBuilder =
                    GetRecruitersForCompanyResponse.newBuilder();

            for (Recruiter recruiter : recruiters) {
                User user = recruiter.getUser();

                RecruiterResponse recruiterResponse = RecruiterResponse.newBuilder()
                        .setId(recruiter.getId())
                        .setEmail(user != null ? safe(user.getEmail()) : "")
                        .setName(user != null ? safe(user.getName()) : "")
                        .setPosition(safe(recruiter.getPosition()))
                        .setHiredById(
                                recruiter.getHiredBy() != null
                                        ? recruiter.getHiredBy().getId()
                                        : 0L
                        )
                        .setWorksInCompanyId(
                                recruiter.getWorksIn() != null
                                        ? recruiter.getWorksIn().getId()
                                        : 0L
                        )
                        .build();

                responseBuilder.addRecruiters(recruiterResponse);
            }

            responseObserver.onNext(responseBuilder.build());
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            responseObserver.onError(e);
        }
    }
    @Override
    @Transactional
    public void getRecruiterById(GetRecruiterByIdRequest request,
                                 StreamObserver<RecruiterResponse> responseObserver) {

        try {
            Recruiter recruiter = recruiterRepository.findById(request.getId()).orElse(null);

            if (recruiter == null) {
                responseObserver.onNext(
                        RecruiterResponse.newBuilder()
                                .setId(0)
                                .setEmail("")
                                .setName("")
                                .setPosition("")
                                .setHiredById(0)
                                .setWorksInCompanyId(0)
                                .build()
                );
                responseObserver.onCompleted();
                return;
            }

            long worksInCompanyId = recruiter.getWorksIn() != null
                    ? recruiter.getWorksIn().getId()
                    : 0;

            RecruiterResponse response = RecruiterResponse.newBuilder()
                    .setId(recruiter.getId())
                    .setEmail(recruiter.getUser().getEmail()) // jeśli chcesz
                    .setName(recruiter.getUser().getName())   // jeśli chcesz
                    .setPosition(recruiter.getPosition() == null ? "" : recruiter.getPosition())
                    .setHiredById(
                            recruiter.getHiredBy() != null ? recruiter.getHiredBy().getId() : 0
                    )
                    .setWorksInCompanyId(worksInCompanyId)
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            responseObserver.onError(e);
        }
    }



    private String safe(String value) {
        return value == null ? "" : value;
    }
}

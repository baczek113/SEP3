package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.UserRole;
import com.example.databaseserver.Repositories.RepresentativeRepository;
import com.example.databaseserver.Repositories.UserRepository;
import com.example.databaseserver.generated.CreateRepRequest;
import com.example.databaseserver.generated.RepresentativeResponse;
import com.example.databaseserver.generated.RepresentativeServiceGrpc;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;

@GrpcService
public class RepresenativeService extends RepresentativeServiceGrpc.RepresentativeServiceImplBase {
    private final RepresentativeRepository repository;
    private final UserRepository userRepository;
    private static final Logger log = LoggerFactory.getLogger(RepresenativeService.class);

    @Autowired
    public RepresenativeService(RepresentativeRepository repository, UserRepository userRepository) {
        this.repository = repository;
        this.userRepository = userRepository;
    }

    @Override
    @Transactional
    public void createRepresentative(CreateRepRequest request, StreamObserver<RepresentativeResponse> responseObserver){
        if (userRepository.existsByEmail(request.getEmail())) {
            log.warn("Blocked duplicate registration attempt for email: {} | REPRESENTATIVE", request.getEmail());
            responseObserver.onError(
                    Status.ALREADY_EXISTS
                            .withDescription("Email already in use: " + request.getEmail())
                            .asRuntimeException()
            );
            return;
        }
        try {
            User user = new User(request.getEmail(), request.getPasswordHash(), UserRole.company_representative, request.getName());
            CompanyRepresentative rep = new CompanyRepresentative(user, request.getPosition());
            CompanyRepresentative savedRep = repository.save(rep);
            RepresentativeResponse response = RepresentativeResponse.newBuilder()
                            .setId(savedRep.getId())
                            .setName(savedRep.getUser().getName())
                            .setEmail(savedRep.getUser().getEmail())
                            .setPosition(savedRep.getPosition())
                            .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }catch (Exception e) {
            responseObserver.onError(Status.INTERNAL
                    .withDescription("Error creating applicant: " + e.getMessage())
                    .asRuntimeException());
        }
    }
}

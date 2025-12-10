package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.UserRole;
import com.example.databaseserver.Repositories.RecruiterRepository;
import com.example.databaseserver.Repositories.RepresentativeRepository;
import com.example.databaseserver.Repositories.UserRepository;
import com.example.databaseserver.generated.CreateRepRequest;
import com.example.databaseserver.generated.RepresentativeResponse;
import com.example.databaseserver.generated.RepresentativeServiceGrpc;
import com.example.databaseserver.generated.UpdateRepRequest;
import com.example.databaseserver.generated.RemoveRepresentativeRequest;
import com.example.databaseserver.generated.RemoveRepresentativeResponse;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.List;
import java.util.Optional;

@GrpcService
public class RepresenativeService extends RepresentativeServiceGrpc.RepresentativeServiceImplBase {
    private final RepresentativeRepository repository;
    private final UserRepository userRepository;
    private static final Logger log = LoggerFactory.getLogger(RepresenativeService.class);
    private final RecruiterRepository recruiterRepository;

    @Autowired
    public RepresenativeService(RepresentativeRepository repository,
                                UserRepository userRepository,
                                RecruiterRepository recruiterRepository) {
        this.repository = repository;
        this.userRepository = userRepository;
        this.recruiterRepository = recruiterRepository;
    }

    @Override
    @Transactional
    public void createRepresentative(CreateRepRequest request, StreamObserver<RepresentativeResponse> responseObserver) {
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
        } catch (Exception e) {
            log.error("Error creating representative", e);
            responseObserver.onError(Status.INTERNAL
                    .withDescription("Error creating representative: " + e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    @Transactional
    public void updateRepresentative(UpdateRepRequest request, StreamObserver<RepresentativeResponse> responseObserver) {
        long id = request.getId();
        try {
            Optional<CompanyRepresentative> optionalRep = repository.findById(id);
            if (optionalRep.isEmpty()) {
                responseObserver.onError(
                        Status.NOT_FOUND
                                .withDescription("Representative not found with id: " + id)
                                .asRuntimeException()
                );
                return;
            }

            CompanyRepresentative rep = optionalRep.get();
            User user = rep.getUser();

            if (!user.getEmail().equals(request.getEmail()) && userRepository.existsByEmail(request.getEmail())) {
                log.warn("Blocked email change to already used email: {} | REPRESENTATIVE {}", request.getEmail(), id);
                responseObserver.onError(
                        Status.ALREADY_EXISTS
                                .withDescription("Email already in use: " + request.getEmail())
                                .asRuntimeException()
                );
                return;
            }

            user.setName(request.getName());
            user.setEmail(request.getEmail());
            if (!request.getPasswordHash().isEmpty()) {
                user.setPassword(request.getPasswordHash());
            }

            rep.setPosition(request.getPosition());

            userRepository.save(user);
            CompanyRepresentative savedRep = repository.save(rep);

            RepresentativeResponse response = RepresentativeResponse.newBuilder()
                    .setId(savedRep.getId())
                    .setName(savedRep.getUser().getName())
                    .setEmail(savedRep.getUser().getEmail())
                    .setPosition(savedRep.getPosition())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            log.error("Error updating representative with id: {}", request.getId(), e);
            responseObserver.onError(
                    Status.INTERNAL
                            .withDescription("Error updating representative: " + e.getMessage())
                            .asRuntimeException()
            );
        }
    }

    @Override
    @Transactional
    public void deleteRepresentative(RemoveRepresentativeRequest request,
                                     StreamObserver<RemoveRepresentativeResponse> responseObserver) {
        long repUserId = request.getId();

        try {
            boolean exists = repository.existsById(repUserId);
            if (!exists) {
                responseObserver.onError(
                        Status.NOT_FOUND
                                .withDescription("Representative not found with id: " + repUserId)
                                .asRuntimeException()
                );
                return;
            }

            List<Long> recruiterUserIds = recruiterRepository
                    .findRecruiterUserIdsByRepresentativeId(repUserId);

            if (!recruiterUserIds.isEmpty()) {
                userRepository.deleteAllById(recruiterUserIds);
            }

            userRepository.deleteById(repUserId);

            RemoveRepresentativeResponse response = RemoveRepresentativeResponse.newBuilder()
                    .setSuccess(true)
                    .setMessage("Representative and related data deleted")
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            log.error("Error deleting representative with id: {}", repUserId, e);
            responseObserver.onError(
                    Status.INTERNAL
                            .withDescription("Error deleting representative: " + e.getMessage())
                            .asRuntimeException()
            );
        }
    }

}

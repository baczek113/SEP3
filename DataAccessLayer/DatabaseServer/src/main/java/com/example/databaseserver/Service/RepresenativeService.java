package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.UserRole;
import com.example.databaseserver.Repositories.RepresentativeRepository;
import com.example.databaseserver.generated.CreateRepRequest;
import com.example.databaseserver.generated.RepresentativeResponse;
import com.example.databaseserver.generated.RepresentativeServiceGrpc;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class RepresenativeService extends RepresentativeServiceGrpc.RepresentativeServiceImplBase {
    private final RepresentativeRepository repository;

    @Autowired
    public RepresenativeService(RepresentativeRepository repository) {
        this.repository = repository;
    }

    @Override
    @Transactional
    public void createRepresentative(CreateRepRequest request, StreamObserver<RepresentativeResponse> responseObserver){
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
        }catch (Exception e){
            responseObserver.onError(e);
        }
    }
}

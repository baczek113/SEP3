package com.example.databaseserver.Service;
import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.UserRole;
import com.example.databaseserver.Repositories.UserRepository;
import com.example.databaseserver.generated.RepresentativeResponse;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import com.example.databaseserver.generated.AuthenticationServiceGrpc;
import com.example.databaseserver.generated.LoginRequest;
import com.example.databaseserver.generated.LoginResponse;


@GrpcService
public class AuthenticationService extends AuthenticationServiceGrpc.AuthenticationServiceImplBase {
    private final UserRepository repository;

    @Autowired
    public AuthenticationService(UserRepository repository) {
        this.repository = repository;
    }

    @Override
    @Transactional
    public void login(LoginRequest request, StreamObserver<LoginResponse> responseObserver){
        try {
            User user = repository.findByEmail(request.getEmail());
            if(!request.getPasswordHash().equals(user.getPassword())) {
                responseObserver.onError(new Exception("Wrong credentials"));
                return;
            }
            LoginResponse response = LoginResponse.newBuilder()
                    .setRole(user.getRole().name())
                    .setName(user.getName())
                    .setId(user.getId())
                    .setEmail(user.getEmail())
                    .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();

        }catch (Exception e){
            responseObserver.onError(e);
        }
    }
}

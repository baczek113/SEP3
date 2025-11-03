package com.example.databaseserver;

import com.example.databaseserver.Service.RepresenativeService;
import io.grpc.Server;
import io.grpc.ServerBuilder;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class GrpcServerConfig {

    @Value("${grpc.server.port:9090}")
    private int port;

    private final RepresenativeService representativeService;

    @Autowired
    public GrpcServerConfig(RepresenativeService representativeService) {
        this.representativeService = representativeService;
    }

    @Bean
    public Server grpcServer() {
        ServerBuilder<?> serverBuilder = ServerBuilder.forPort(port)
                .addService(representativeService);

        Server server = serverBuilder.build();

        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            if (server != null && !server.isShutdown()) {
                System.err.println("Shutting down gRPC server (JVM shutdown)");
                server.shutdown();
                System.err.println("gRPC server shut down.");
            }
        }));

        return server;
    }
}


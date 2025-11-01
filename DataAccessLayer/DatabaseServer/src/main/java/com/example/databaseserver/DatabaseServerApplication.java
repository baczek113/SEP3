package com.example.databaseserver;

import io.grpc.Server;
import org.springframework.boot.WebApplicationType;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.context.ConfigurableApplicationContext;

import java.io.IOException;

@SpringBootApplication
public class DatabaseServerApplication {

    public static void main(String[] args) throws IOException, InterruptedException {
        ConfigurableApplicationContext context = new SpringApplicationBuilder(DatabaseServerApplication.class)
                .web(WebApplicationType.NONE)
                .run(args);

        Server server = context.getBean(Server.class);

        server.start();
        System.out.println("gRPC Server started, listening on " + server.getPort());

        server.awaitTermination();
    }

}

package com.example.databaseserver;

import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.UserRole;
import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.TestPropertySource;

import static org.junit.jupiter.api.Assertions.*;

@SpringBootTest
@TestPropertySource(properties = {
        "spring.datasource.url=jdbc:h2:mem:testdb",     // Test DB (in-memory)
        "spring.jpa.hibernate.ddl-auto=create-drop",    // Reset schema for tests
        "grpc.server.port=0"                            // Disable real gRPC port
})
class DatabaseServerApplicationTests {

    @Test
    void contextLoads() {
        // Test checks only that Spring starts.
    }

    @Test
    void testUserWithName() {
        User user = new User(
                "test@example.com",
                "password123",
                UserRole.applicant,
                "John Doe"
        );

        assertEquals("John Doe", user.getName());
    }

    @Test
    void testUserWithNullName() {
        User user = new User(
                "test@example.com",
                "password123",
                UserRole.applicant,
                null
        );

        assertNull(user.getName());
    }
}

package com.example.databaseserver.Entities;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class UserTest {

    // -----------------------------------
    // ID tests
    // -----------------------------------

    @Test
    void testZeroId() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "John Doe");
        user.setId(0L);
        assertEquals(0L, user.getId());
    }

    @Test
    void testOneId() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "John Doe");
        user.setId(1L);
        assertEquals(1L, user.getId());
    }

    @Test
    void testManyId() {
        User user1 = new User("a@example.com", "p1", UserRole.applicant, "Alice");
        User user2 = new User("b@example.com", "p2", UserRole.applicant, "Bob");
        User user3 = new User("c@example.com", "p3", UserRole.applicant, "Charlie");

        user1.setId(1L);
        user2.setId(2L);
        user3.setId(3L);

        assertAll("id",
                () -> assertEquals(1L, user1.getId()),
                () -> assertEquals(2L, user2.getId()),
                () -> assertEquals(3L, user3.getId())
        );
    }

    @Test
    void testBoundaryId() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "John Doe");
        user.setId(Long.MAX_VALUE);
        assertEquals(Long.MAX_VALUE, user.getId());
    }

    @Test
    void testInitialIdIsNull() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "John Doe");
        assertNull(user.getId());
    }

    // -----------------------------------
    // Email tests
    // -----------------------------------

    @Test
    void testZeroEmail() {
        User user = new User("", "password123", UserRole.applicant, "John Doe");
        assertEquals("", user.getEmail());
    }

    @Test
    void testOneEmail() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "John Doe");
        assertEquals("test@example.com", user.getEmail());
    }

    @Test
    void testManyEmail() {
        User user1 = new User("a@example.com", "p1", UserRole.applicant, "Alice");
        User user2 = new User("b@example.com", "p2", UserRole.applicant, "Bob");
        User user3 = new User("c@example.com", "p3", UserRole.applicant, "Charlie");

        assertAll("email",
                () -> assertEquals("a@example.com", user1.getEmail()),
                () -> assertEquals("b@example.com", user2.getEmail()),
                () -> assertEquals("c@example.com", user3.getEmail())
        );
    }

    @Test
    void testBoundaryEmail() {
        String longEmail = "a".repeat(200) + "@example.com";
        User user = new User(longEmail, "password123", UserRole.applicant, "John Doe");
        assertEquals(longEmail, user.getEmail());
    }

    @Test
    void testNullEmail() {
        User user = new User(null, "password123", UserRole.applicant, "John Doe");
        assertNull(user.getEmail());
    }

    // -----------------------------------
    // Password tests
    // -----------------------------------

    @Test
    void testZeroPassword() {
        User user = new User("test@example.com", "", UserRole.applicant, "John Doe");
        assertEquals("", user.getPassword());
    }

    @Test
    void testOnePassword() {
        User user = new User("test@example.com", "secret123", UserRole.applicant, "John Doe");
        assertEquals("secret123", user.getPassword());
    }

    @Test
    void testManyPassword() {
        User user1 = new User("a@example.com", "p1", UserRole.applicant, "Alice");
        User user2 = new User("b@example.com", "p2", UserRole.applicant, "Bob");
        User user3 = new User("c@example.com", "p3", UserRole.applicant, "Charlie");

        assertAll("password",
                () -> assertEquals("p1", user1.getPassword()),
                () -> assertEquals("p2", user2.getPassword()),
                () -> assertEquals("p3", user3.getPassword())
        );
    }

    @Test
    void testBoundaryPassword() {
        String longPassword = "A".repeat(1000);
        User user = new User("test@example.com", longPassword, UserRole.applicant, "John Doe");
        assertEquals(longPassword, user.getPassword());
    }

    @Test
    void testNullPassword() {
        User user = new User("test@example.com", null, UserRole.applicant, "John Doe");
        assertNull(user.getPassword());
    }

    // -----------------------------------
    // Role tests
    // -----------------------------------

    @Test
    void testOneRole() {
        User user = new User("test@example.com", "password123", UserRole.recruiter, "John Doe");
        assertEquals(UserRole.recruiter, user.getRole());
    }

    @Test
    void testManyRole() {
        User user1 = new User("a@example.com", "p1", UserRole.applicant, "Alice");
        User user2 = new User("b@example.com", "p2", UserRole.recruiter, "Bob");
        User user3 = new User("c@example.com", "p3", UserRole.admin, "Charlie");

        assertAll("role",
                () -> assertEquals(UserRole.applicant, user1.getRole()),
                () -> assertEquals(UserRole.recruiter, user2.getRole()),
                () -> assertEquals(UserRole.admin, user3.getRole())
        );
    }

    @Test
    void testNullRole() {
        User user = new User("test@example.com", "password123", null, "John Doe");
        assertNull(user.getRole());
    }

    // -----------------------------------
    // Name tests
    // -----------------------------------

    @Test
    void testZeroName() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "");
        assertEquals("", user.getName());
    }

    @Test
    void testOneName() {
        User user = new User("test@example.com", "password123", UserRole.applicant, "John Doe");
        assertEquals("John Doe", user.getName());
    }

    @Test
    void testManyName() {
        User user1 = new User("a@example.com", "p1", UserRole.applicant, "Alice");
        User user2 = new User("b@example.com", "p2", UserRole.applicant, "Bob");
        User user3 = new User("c@example.com", "p3", UserRole.applicant, "Charlie");

        assertAll("name",
                () -> assertEquals("Alice", user1.getName()),
                () -> assertEquals("Bob", user2.getName()),
                () -> assertEquals("Charlie", user3.getName())
        );
    }

    @Test
    void testBoundaryName() {
        String longName = "A".repeat(500);
        User user = new User("test@example.com", "password123", UserRole.applicant, longName);
        assertEquals(longName, user.getName());
    }

    @Test
    void testNullName() {
        User user = new User("test@example.com", "password123", UserRole.applicant, null);
        assertNull(user.getName());
    }
}

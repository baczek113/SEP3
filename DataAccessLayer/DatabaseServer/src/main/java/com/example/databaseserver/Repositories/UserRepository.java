package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.User;
import org.springframework.data.jpa.repository.JpaRepository;

public interface UserRepository extends JpaRepository<User, Long>{
    User findByEmail(String username);
}

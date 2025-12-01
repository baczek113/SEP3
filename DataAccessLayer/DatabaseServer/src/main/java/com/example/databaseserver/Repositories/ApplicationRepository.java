package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Application;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ApplicationRepository extends JpaRepository<Application, Long> {
}

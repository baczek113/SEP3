package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Applicant;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ApplicantRepository extends JpaRepository<Applicant, Long> {
}

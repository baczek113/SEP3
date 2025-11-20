package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Recruiter;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface RecruiterRepository extends JpaRepository<Recruiter, Long> {


    List<Recruiter> findByWorksIn_Id(Long companyId);


    List<Recruiter> findByHiredBy_Id(Long representativeId);
}

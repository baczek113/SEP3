package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Application;
import com.example.databaseserver.Entities.JobSkill;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ApplicationRepository extends JpaRepository<Application, Long> {
    List<Application> findApplicationsByJob_Id(long jobId);
    List<Application> findApplicationsByApplicant_Id(long applicantId);
    void deleteByApplicantId(long applicantId);

}

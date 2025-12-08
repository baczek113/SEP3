package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.ApplicantSkill;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ApplicantSkillRepository extends JpaRepository<ApplicantSkill, Long> {

    List<ApplicantSkill> findByApplicant_Id(Long applicantId);
    boolean existsByApplicant_IdAndSkill_Id(Long applicantId, Long skillId);
    void deleteByApplicantId(long applicantId);

}

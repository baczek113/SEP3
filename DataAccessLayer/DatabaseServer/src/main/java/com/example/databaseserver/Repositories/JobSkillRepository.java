package com.example.databaseserver.Repositories;


import com.example.databaseserver.Entities.JobSkill;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface JobSkillRepository extends JpaRepository<JobSkill, Long> {
    List<JobSkill> findJobSkillsByJobId(long jobListingId);
}

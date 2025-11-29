package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

@Entity
@Table(name = "job_listing_skills")
@Data
@NoArgsConstructor
@IdClass(JobSkillId.class)
public class JobSkill {

    @Id
    private Long jobId;

    @Id
    private Long skillId;

    @Column(nullable = false)
    private String priority;
}

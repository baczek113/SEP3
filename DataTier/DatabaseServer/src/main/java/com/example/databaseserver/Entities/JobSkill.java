package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "job_listing_skills", schema = "hirefire")
@Data
@NoArgsConstructor
public class JobSkill {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "job_id", nullable = false)
    private JobListing jobListing;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "skill_id", nullable = false)
    private Skill skill;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 10)
    private JobSkillPriority priority;

    public JobSkill(JobListing jobListing, Skill skill, JobSkillPriority priority) {
        this.jobListing = jobListing;
        this.skill = skill;
        this.priority = priority;
    }

    public Long getId()                { return id; }
    public JobListing getJobListing()  { return jobListing; }
    public Skill getSkill()            { return skill; }
    public JobSkillPriority getPriority() { return priority; }
}

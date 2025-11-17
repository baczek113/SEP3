package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "applicant_skill", schema = "hirefire")
@Data
@NoArgsConstructor
public class ApplicantSkill {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "applicant_id", nullable = false)
    private Applicant applicant;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "skill_id", nullable = false)
    private Skill skill;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private SkillLevel level;

    public ApplicantSkill(Applicant applicant, Skill skill, SkillLevel level) {
        this.applicant = applicant;
        this.skill = skill;
        this.level = level;
    }

    public Long getId() {
        return id;
    }

    public Applicant getApplicant() {
        return applicant;
    }

    public Skill getSkill() {
        return skill;
    }

    public SkillLevel getLevel() {
        return level;
    }
}

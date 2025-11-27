package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "recruiter")
@Data
@NoArgsConstructor
public class Recruiter {

    @Id
    @Column(name = "user_id")
    private Long id;

    @OneToOne(fetch = FetchType.LAZY)
    @MapsId
    @JoinColumn(name = "user_id")
    private User user;

    @Column
    private String position;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "hired_by")
    private CompanyRepresentative hiredBy;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "works_in")
    private Company worksIn;

    public Recruiter(User user, String position, CompanyRepresentative hiredBy, Company worksIn) {
        this.user = user;
        this.position = position;
        this.hiredBy = hiredBy;
        this.worksIn = worksIn;
    }
}

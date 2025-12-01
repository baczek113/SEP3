package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

@Entity
@Table(name = "job_listing")
@Data
@NoArgsConstructor
public class JobListing {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 255)
    private String title;

    @Column(columnDefinition = "text")
    private String description;

    @Column(name = "date_posted", nullable = false)
    private OffsetDateTime datePosted;

    @Column(precision = 12, scale = 2)
    private BigDecimal salary;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "company_id", nullable = false)
    private Company company;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "location_id", nullable = false)
    private Location location;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "posted_by_id")
    private Recruiter postedBy;

    // ---- Custom constructor ----
    public JobListing(String title,
                      String description,
                      OffsetDateTime datePosted,
                      BigDecimal salary,
                      Company company,
                      Location location,
                      Recruiter postedBy) {

        this.title = title;
        this.description = description;
        this.datePosted = datePosted;
        this.salary = salary;
        this.company = company;
        this.location = location;
        this.postedBy = postedBy;
    }
}

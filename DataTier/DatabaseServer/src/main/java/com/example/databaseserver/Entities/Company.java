package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "company")
@Data
@NoArgsConstructor
public class Company {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 255)
    private String name;

    @Column(columnDefinition = "text")
    private String description;

    @Column(length = 255)
    private String website;

    @Column(name = "is_approved", nullable = false)
    private boolean approved = false;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "cr_id")
    private CompanyRepresentative companyRepresentative;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "location_id")
    private Location location;

    public Company(String name,
                   String description,
                   String website,
                   CompanyRepresentative companyRepresentative,
                   Location location)
    {
        this.name = name;
        this.description = description;
        this.website = website;
        this.companyRepresentative = companyRepresentative;
        this.location = location;
        this.approved = false;
    }

    public Long getId() {
        return id;
    }
}

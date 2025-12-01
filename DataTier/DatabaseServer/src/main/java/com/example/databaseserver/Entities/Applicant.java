package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "applicant")
@Data
@NoArgsConstructor
public class Applicant {

    @Id
    private Long id;

    @OneToOne(fetch = FetchType.LAZY, cascade = CascadeType.ALL)
    @MapsId
    @JoinColumn(name = "user_id")
    private User user;

    @Column(columnDefinition = "text")
    private String experience;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "location_id")
    private Location location;

    public Applicant(User user, String experience, Location location) {
        this.user = user;
        this.experience = experience;
        this.location = location;
    }

    public Long getId() {
        return id;
    }

    public String getExperience() {
        return experience;
    }

    public User getUser() {
        return user;
    }

    public Location getLocation() {
        return location;
    }
}

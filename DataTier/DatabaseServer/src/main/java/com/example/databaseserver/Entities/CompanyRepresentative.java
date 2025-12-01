package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "company_representative")
@Data
@NoArgsConstructor
public class CompanyRepresentative {
    @Id
    private Long id;

    @OneToOne(fetch = FetchType.LAZY, cascade = CascadeType.ALL)
    @MapsId
    @JoinColumn(name = "user_id")
    private User user;

    @Column
    private String position;

    public CompanyRepresentative(User user , String position) {
        this.user = user;
        this.position = position;
    }

    public Long getId() {
        return id;
    }

    public String getPosition() {
        return position;
    }

    public User getUser() {
        return user;
    }
}

package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "chat_thread", schema = "hirefire")
@Data
@NoArgsConstructor
public class ChatThread {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "chat_id")
    private Long id;

    @OneToOne
    @JoinColumn(name = "applicant_id")
    private Applicant applicant;

    public ChatThread(Applicant applicant) {
        this.applicant = applicant;
    }
}

package com.example.databaseserver.Entities;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.OffsetDateTime;

@Entity
@Table(name = "message", schema = "hirefire")
@Data
@NoArgsConstructor
public class Message {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "chat_id")
    private ChatThread chatThread;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "sender_id")
    private User sender;

    @Column(columnDefinition = "TEXT")
    private String body;

    @Column(name = "sent_at")
    private OffsetDateTime sentAt;

    public Message(ChatThread chatThread, User sender, String body, OffsetDateTime sentAt) {
        this.chatThread = chatThread;
        this.sender = sender;
        this.body = body;
        this.sentAt = sentAt;
    }
}

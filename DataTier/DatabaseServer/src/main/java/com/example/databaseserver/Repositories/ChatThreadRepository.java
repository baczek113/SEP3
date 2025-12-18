package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.ChatThread;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ChatThreadRepository extends JpaRepository<ChatThread, Long> {
    ChatThread findByApplication_Id(Long applicantId);
}

package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.ChatThread;
import com.example.databaseserver.Entities.Message;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface MessageRepository extends JpaRepository<Message, Long> {
    List<Message> findByChatThreadOrderBySentAtAsc(ChatThread chatThread);
}

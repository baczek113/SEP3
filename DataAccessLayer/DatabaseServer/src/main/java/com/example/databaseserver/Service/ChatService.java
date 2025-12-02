package com.example.databaseserver.Service;
import com.example.databaseserver.Entities.ChatThread;
import com.example.databaseserver.Entities.Message;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Repositories.ChatThreadRepository;
import com.example.databaseserver.Repositories.MessageRepository;
import com.example.databaseserver.Repositories.UserRepository;
import com.example.databaseserver.generated.ChatServiceGrpc;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import com.example.databaseserver.generated.SaveMessagesRequest;
import com.example.databaseserver.generated.SaveMessagesResponse;
import com.example.databaseserver.generated.ChatMessage;
import com.example.databaseserver.generated.GetMessagesRequest;
import com.example.databaseserver.generated.GetMessagesResponse;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;

@GrpcService
public class ChatService extends ChatServiceGrpc.ChatServiceImplBase {
    private final ChatThreadRepository chatThreadRepository;
    private final MessageRepository messageRepository;
    private final UserRepository userRepository;

    @Autowired
    public ChatService(ChatThreadRepository chatThreadRepository, MessageRepository messageRepository, UserRepository userRepository) {
        this.chatThreadRepository = chatThreadRepository;
        this.messageRepository = messageRepository;
        this.userRepository = userRepository;
    }

    @Override
    @Transactional
    public void getMessages(GetMessagesRequest request, StreamObserver<GetMessagesResponse> responseObserver){
        try {
            GetMessagesResponse.Builder builder = GetMessagesResponse.newBuilder();

            ChatThread thread = chatThreadRepository.findByApplicationId(request.getApplicationId());

            if (thread != null){
                List<Message> messages = messageRepository.findByChatThreadOrderBySentAtAsc(thread);
                DateTimeFormatter formatter = DateTimeFormatter.ofPattern("HH:mm");

                for (Message msh : messages){
                    ChatMessage protoMsg = ChatMessage.newBuilder()
                            .setSenderName(msh.getSender().getName())
                            .setSenderId(msh.getSender().getId())
                            .setBody(msh.getBody())
                            .setSendAt(msh.getSentAt().format(formatter))
                            .build();
                    builder.addMessages(protoMsg);
                }
                responseObserver.onNext(builder.build());
                responseObserver.onCompleted();
            }
        }catch (Exception e){
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void save(SaveMessagesRequest request, StreamObserver<SaveMessagesResponse> responseObserver ){
        try {
            ChatThread thread = chatThreadRepository.findByApplicationId(request.getApplicationId());

            if (thread == null) {
                throw new RuntimeException("Chat thread does not exist.");
            }

            User sender = userRepository.findById(request.getSenderId()).orElseThrow(() -> new RuntimeException("Sender does not exist."));

            Message msg = new Message(thread, sender, request.getBody(), OffsetDateTime.now());
            messageRepository.save(msg);
            DateTimeFormatter formatter = DateTimeFormatter.ofPattern("HH:mm");

            ChatMessage createdMessage = ChatMessage.newBuilder()
                    .setSenderName(sender.getName())
                    .setSenderId(sender.getId())
                    .setBody(msg.getBody())
                    .setSendAt(msg.getSentAt().format(formatter))
                    .build();

            SaveMessagesResponse response = SaveMessagesResponse.newBuilder()
                    .setSuccess(true)
                    .setCreatedMessage(createdMessage)
                    .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();

        }catch (Exception e){
            responseObserver.onError(e);
        }
    }

}
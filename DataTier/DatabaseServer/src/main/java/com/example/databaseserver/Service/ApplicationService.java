package com.example.databaseserver.Service;


import com.example.databaseserver.Entities.*;
import com.example.databaseserver.Entities.Applicant;
import com.example.databaseserver.Entities.Application;
import com.example.databaseserver.Entities.ApplicationStatus;
import com.example.databaseserver.Repositories.ApplicantRepository;
import com.example.databaseserver.Repositories.ApplicationRepository;
import com.example.databaseserver.Repositories.ChatThreadRepository;
import com.example.databaseserver.Repositories.JobListingRepository;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import com.example.databaseserver.generated.ApplicationServiceGrpc.*;
import com.example.databaseserver.generated.ChangeStatusRequest;
import com.example.databaseserver.generated.CreateApplicationRequest;
import com.example.databaseserver.generated.GetApplicationsForApplicantRequest;
import com.example.databaseserver.generated.GetApplicationsForJobRequest;
import com.example.databaseserver.generated.ApplicationsResponse;
import com.example.databaseserver.generated.ApplicationResponse;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;

@GrpcService
public class ApplicationService extends com.example.databaseserver.generated.ApplicationServiceGrpc.ApplicationServiceImplBase{

    private final JobListingRepository jobListingRepository;
    private final ApplicantRepository applicantRepository;
    private final ApplicationRepository applicationRepository;
    private final ChatThreadRepository chatThreadRepository;

    @Autowired
    public ApplicationService(JobListingRepository jobListingRepository, ApplicantRepository applicantRepository, ApplicationRepository applicationRepository, ChatThreadRepository chatThreadRepository) {
        this.jobListingRepository = jobListingRepository;
        this.applicantRepository = applicantRepository;
        this.applicationRepository = applicationRepository;
        this.chatThreadRepository = chatThreadRepository;
    }

    @Override
    @Transactional
    public void createApplication(CreateApplicationRequest request,
                                  StreamObserver<ApplicationResponse> responseObserver) {
        try {
            JobListing jobListing = jobListingRepository.findById(request.getJobId())
                    .orElseThrow(() -> new RuntimeException("Job listing not found"));

            Applicant applicant = applicantRepository.findById(request.getApplicantId())
                    .orElseThrow(() -> new RuntimeException("Applicant not found"));

            OffsetDateTime now = OffsetDateTime.now();

            Application application = new Application(jobListing, applicant, now, ApplicationStatus.under_review);

            Application saved = applicationRepository.save(application);

            ApplicationResponse response = ApplicationResponse.newBuilder()
                    .setId(saved.getId())
                    .setJobId(jobListing.getId())
                    .setApplicantId(applicant.getId())
                    .setSubmittedAt(now.toString())
                    .setStatus(com.example.databaseserver.generated.ApplicationStatus.UNDER_REVIEW)
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            e.printStackTrace();
            responseObserver.onError(
                    Status.UNKNOWN
                            .withDescription(e.getMessage())
                            .withCause(e)
                            .asRuntimeException()
            );
        }
    }

    @Override
    @Transactional
    public void getApplicationsForJob(GetApplicationsForJobRequest request,
                                    StreamObserver<ApplicationsResponse> responseObserver) {
        try {
            List<Application> applications = applicationRepository.findApplicationsByJob_Id(request.getJobId());
            List<ApplicationResponse> response = new ArrayList<>();

            for(Application application : applications) {
                ApplicationResponse applicationResponse = ApplicationResponse.newBuilder()
                        .setId(application.getId())
                        .setJobId(application.getJob().getId())
                        .setApplicantId(application.getApplicant().getId())
                        .setStatus(mapEntityToProtoStatus(application.getStatus()))
                        .setSubmittedAt(application.getSubmittedAt().toString())
                        .build();
                response.add(applicationResponse);
            }

            responseObserver.onNext(ApplicationsResponse.newBuilder().addAllApplications(response).build());
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void getApplicationsForApplicant(GetApplicationsForApplicantRequest request,
                                      StreamObserver<ApplicationsResponse> responseObserver) {
        try {
            List<Application> applications = applicationRepository.findApplicationsByApplicant_Id(request.getApplicantId());
            List<ApplicationResponse> response = new ArrayList<>();

            for(Application application : applications) {
                ApplicationResponse applicationResponse = ApplicationResponse.newBuilder()
                        .setId(application.getId())
                        .setJobId(application.getJob().getId())
                        .setApplicantId(application.getApplicant().getId())
                        .setStatus(mapEntityToProtoStatus(application.getStatus()))
                        .setSubmittedAt(application.getSubmittedAt().toString())
                        .build();
                response.add(applicationResponse);
            }

            responseObserver.onNext(ApplicationsResponse.newBuilder().addAllApplications(response).build());
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void acceptApplication(ChangeStatusRequest request,
                                StreamObserver<ApplicationResponse> responseObserver)
    {
        try {
            changeApplicationStatusHelper(request, ApplicationStatus.matched, responseObserver);
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void rejectApplication(ChangeStatusRequest request,
                                StreamObserver<ApplicationResponse> responseObserver)
    {
        try {
            changeApplicationStatusHelper(request, ApplicationStatus.declined, responseObserver);
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    private void changeApplicationStatusHelper(ChangeStatusRequest request, ApplicationStatus status, StreamObserver<ApplicationResponse> responseObserver)
    {
        Application application = applicationRepository.findById(request.getApplicationId())
                .orElseThrow(() -> new RuntimeException("Application not found"));

        application.setStatus(status);
        applicationRepository.save(application);

        if(status == ApplicationStatus.matched && chatThreadRepository.findByApplication_Id(application.getId()) == null){
            ChatThread chatThread = new ChatThread(application);
            chatThreadRepository.save(chatThread);
        }

        ApplicationResponse response = ApplicationResponse.newBuilder()
                .setId(application.getId())
                .setJobId(application.getJob().getId())
                .setApplicantId(application.getApplicant().getId())
                .setStatus(mapEntityToProtoStatus(application.getStatus()))
                .setSubmittedAt(application.getSubmittedAt().toString())
                .build();
        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    private com.example.databaseserver.generated.ApplicationStatus mapEntityToProtoStatus(ApplicationStatus status) {
        return switch (status) {
            case under_review -> com.example.databaseserver.generated.ApplicationStatus.UNDER_REVIEW;
            case declined -> com.example.databaseserver.generated.ApplicationStatus.DECLINED;
            case matched -> com.example.databaseserver.generated.ApplicationStatus.MATCHED;
        };
    }
}

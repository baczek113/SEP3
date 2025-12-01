package com.example.databaseserver.Service;


import com.example.databaseserver.Entities.*;
import com.example.databaseserver.Entities.Applicant;
import com.example.databaseserver.Entities.Application;
import com.example.databaseserver.Entities.ApplicationStatus;
import com.example.databaseserver.Repositories.ApplicantRepository;
import com.example.databaseserver.Repositories.ApplicationRepository;
import com.example.databaseserver.Repositories.JobListingRepository;
import com.example.databaseserver.generated.*;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;

@GrpcService
public class ApplicationService extends ApplicationServiceGrpc.ApplicationServiceImplBase{

    private final JobListingRepository jobListingRepository;
    private final ApplicantRepository applicantRepository;
    private final ApplicationRepository applicationRepository;

    @Autowired
    public ApplicationService(JobListingRepository jobListingRepository, ApplicantRepository applicantRepository, ApplicationRepository applicationRepository) {
        this.jobListingRepository = jobListingRepository;
        this.applicantRepository = applicantRepository;
        this.applicationRepository = applicationRepository;
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

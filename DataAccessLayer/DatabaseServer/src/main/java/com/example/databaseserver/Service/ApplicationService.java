package com.example.databaseserver.Service;


import com.example.databaseserver.Entities.Applicant;
import com.example.databaseserver.Entities.ApplicationStatus;
import com.example.databaseserver.Entities.JobListing;
import com.example.databaseserver.Entities.Application;
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
}

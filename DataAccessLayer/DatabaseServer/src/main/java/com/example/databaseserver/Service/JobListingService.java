package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.Company;
import com.example.databaseserver.Entities.JobSkill;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Entities.JobListing;
import com.example.databaseserver.Entities.Recruiter;
import com.example.databaseserver.Repositories.*;
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
public class JobListingService extends JobListingServiceGrpc.JobListingServiceImplBase {

    private final JobListingRepository jobListingRepository;
    private final CompanyRepository companyRepository;
    private final LocationRepository locationRepository;
    private final RecruiterRepository recruiterRepository;
    private final JobSkillRepository jobSkillRepository;

    @Autowired
    public JobListingService(JobListingRepository jobListingRepository,
                             CompanyRepository companyRepository,
                             LocationRepository locationRepository,
                             RecruiterRepository recruiterRepository,
                             JobSkillRepository jobSkillRepository) {
        this.jobListingRepository = jobListingRepository;
        this.companyRepository = companyRepository;
        this.locationRepository = locationRepository;
        this.recruiterRepository = recruiterRepository;
        this.jobSkillRepository = jobSkillRepository;
    }

    @Override
    @Transactional
    public void createJobListing(CreateJobListingRequest request,
                                 StreamObserver<JobListingResponse> responseObserver) {
        try {
            Company company = companyRepository.findById(request.getCompanyId())
                    .orElseThrow(() -> new RuntimeException("Company not found"));

            Recruiter recruiter = recruiterRepository.findById(request.getPostedById())
                    .orElseThrow(() -> new RuntimeException("Recruiter not found"));

            Location location = new Location(
                    request.getCity(),
                    request.getPostcode(),
                    request.getAddress()
            );


            Location savedLocation = locationRepository.save(location);

            BigDecimal salary = null;
            if (!request.getSalary().isBlank()) {
                salary = new BigDecimal(request.getSalary());
            }


            OffsetDateTime now = OffsetDateTime.now();

            JobListing job = new JobListing(
                    request.getTitle(),
                    request.getDescription(),
                    now,
                    salary,
                    company,
                    savedLocation,
                    recruiter
            );

            JobListing saved = jobListingRepository.save(job);


            JobListingResponse response = JobListingResponse.newBuilder()
                    .setId(saved.getId())
                    .setTitle(saved.getTitle())
                    .setDescription(saved.getDescription() == null ? "" : saved.getDescription())
                    .setCompanyId(saved.getCompany().getId())
                    .setPostedById(saved.getPostedBy().getId())
                    .setDatePosted(saved.getDatePosted().toString())
                    .setSalary(salary != null ? salary.toPlainString() : "")
                    .setCity(savedLocation.getCity())
                    .setPostcode(savedLocation.getPostcode() == null ? "" : savedLocation.getPostcode())
                    .setAddress(savedLocation.getAddress() == null ? "" : savedLocation.getAddress())
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
    public void getJobListingsForCompany(GetJobListingsForCompanyRequest request,
                                         StreamObserver<GetJobListingsForCompanyResponse> responseObserver) {

        try {
            List<JobListing> listings = jobListingRepository.findByCompany_Id(request.getCompanyId());

            GetJobListingsForCompanyResponse.Builder builder =
                    GetJobListingsForCompanyResponse.newBuilder();

            for (JobListing job : listings) {

                JobListingResponse jobResponse = JobListingResponse.newBuilder()
                        .setId(job.getId())
                        .setTitle(safe(job.getTitle()))
                        .setDescription(safe(job.getDescription()))
                        .setDatePosted(job.getDatePosted().toString())
                        .setSalary(job.getSalary() != null ? job.getSalary().toPlainString() : "")
                        .setCompanyId(job.getCompany().getId())
                        .setCity(job.getLocation().getCity())
                        .setPostcode(
                                job.getLocation().getPostcode() == null
                                        ? ""
                                        : job.getLocation().getPostcode()
                        )
                        .setAddress(
                                job.getLocation().getAddress() == null
                                        ? ""
                                        : job.getLocation().getAddress()
                        )
                        .setPostedById(job.getPostedBy() != null ? job.getPostedBy().getId() : 0L)
                        .build();

                builder.addJobListings(jobResponse);
            }

            responseObserver.onNext(builder.build());
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }


    @Override
    @Transactional
    public void getJobListingsForRecruiter(GetJobListingsForRecruiterRequest request,
                                           StreamObserver<GetJobListingsForRecruiterResponse> responseObserver) {
        try {
            List<JobListing> listings = jobListingRepository.findByPostedBy_Id(request.getRecruiterId());

            GetJobListingsForRecruiterResponse.Builder builder =
                    GetJobListingsForRecruiterResponse.newBuilder();

            for (JobListing job : listings) {

                JobListingResponse jobResponse = JobListingResponse.newBuilder()
                        .setId(job.getId())
                        .setTitle(safe(job.getTitle()))
                        .setDescription(safe(job.getDescription()))
                        .setDatePosted(job.getDatePosted().toString())
                        .setSalary(job.getSalary() != null ? job.getSalary().toPlainString() : "")
                        .setCompanyId(job.getCompany().getId())
                        .setCity(job.getLocation().getCity())
                        .setPostcode(
                                job.getLocation().getPostcode() == null
                                        ? ""
                                        : job.getLocation().getPostcode()
                        )
                        .setAddress(
                                job.getLocation().getAddress() == null
                                        ? ""
                                        : job.getLocation().getAddress()
                        )
                        .setPostedById(job.getPostedBy() != null ? job.getPostedBy().getId() : 0L)
                        .build();

                builder.addJobListings(jobResponse);
            }

            responseObserver.onNext(builder.build());
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void getJobListingSkills(GetJobListingSkillsRequest request,
                                           StreamObserver<GetJobListingSkillsResponse> responseObserver) {
        try {
            List<JobSkill> jobSkills = jobSkillRepository.findJobSkillsByJobId(request.getJobListingId());
            List<JobSkillResponse> response = new ArrayList<>();

            for(JobSkill jobSkill : jobSkills) {
                JobSkillResponse jobSkillResponse = JobSkillResponse.newBuilder()
                        .setId(jobSkill.getSkillId())
                        .setPriority(jobSkill.getPriority())
                        .build();
                response.add(jobSkillResponse);
            }

            responseObserver.onNext(GetJobListingSkillsResponse.newBuilder().addAllSkills(response).build());
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    private String safe(String val) {
        return val == null ? "" : val;
    }
}

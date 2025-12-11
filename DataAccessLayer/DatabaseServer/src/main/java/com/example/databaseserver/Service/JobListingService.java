package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.Company;
import com.example.databaseserver.Entities.JobListing;
import com.example.databaseserver.Entities.JobSkill;
import com.example.databaseserver.Entities.JobSkillPriority;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Entities.Recruiter;
import com.example.databaseserver.Entities.Skill;
import com.example.databaseserver.Repositories.CompanyRepository;
import com.example.databaseserver.Repositories.JobListingRepository;
import com.example.databaseserver.Repositories.JobSkillRepository;
import com.example.databaseserver.Repositories.LocationRepository;
import com.example.databaseserver.Repositories.RecruiterRepository;
import com.example.databaseserver.Repositories.SkillRepository;
import com.example.databaseserver.generated.AddJobListingSkillRequest;
import com.example.databaseserver.generated.CreateJobListingRequest;
import com.example.databaseserver.generated.GetJobListingSkillsRequest;
import com.example.databaseserver.generated.GetJobListingSkillsResponse;
import com.example.databaseserver.generated.GetJobListingsByCityRequest;
import com.example.databaseserver.generated.GetJobListingsForCompanyRequest;
import com.example.databaseserver.generated.GetJobListingsForCompanyResponse;
import com.example.databaseserver.generated.GetJobListingsForRecruiterRequest;
import com.example.databaseserver.generated.GetJobListingsForRecruiterResponse;
import com.example.databaseserver.generated.GetJobListingByIdRequest;
import com.example.databaseserver.generated.JobListingResponse;
import com.example.databaseserver.generated.JobListingServiceGrpc;
import com.example.databaseserver.generated.JobListingsResponse;
import com.example.databaseserver.generated.JobSkillResponse;
import com.example.databaseserver.generated.UpdateJobListingRequest;
import com.example.databaseserver.generated.CloseJobListingRequest;
import com.example.databaseserver.generated.RemoveJobListingRequest;
import com.example.databaseserver.generated.RemoveJobListingResponse;
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
    private final SkillRepository skillRepository;

    @Autowired
    public JobListingService(JobListingRepository jobListingRepository,
                             CompanyRepository companyRepository,
                             LocationRepository locationRepository,
                             RecruiterRepository recruiterRepository,
                             JobSkillRepository jobSkillRepository,
                             SkillRepository skillRepository) {
        this.jobListingRepository = jobListingRepository;
        this.companyRepository = companyRepository;
        this.locationRepository = locationRepository;
        this.recruiterRepository = recruiterRepository;
        this.jobSkillRepository = jobSkillRepository;
        this.skillRepository = skillRepository;
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
                    recruiter,
                    request.getIsClosed()
            );

            JobListing saved = jobListingRepository.save(job);

            JobListingResponse response = mapToResponse(saved);

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
    public void removeJobListing(RemoveJobListingRequest request,
                                 StreamObserver<RemoveJobListingResponse> responseObserver)
    {
        try {
            long id = request.getId();

            boolean exists = jobListingRepository.existsById(id);

            if (!exists) {
                RemoveJobListingResponse response = RemoveJobListingResponse.newBuilder()
                        .setSuccess(false)
                        .setMessage("Job listing not found.")
                        .build();

                responseObserver.onNext(response);
                responseObserver.onCompleted();
                return;
            }

            jobListingRepository.deleteById(id);

            RemoveJobListingResponse response = RemoveJobListingResponse.newBuilder()
                    .setSuccess(true)
                    .setMessage("Job listing deleted successfully.")
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
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

                JobListingResponse jobResponse = mapToResponse(job);

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

                JobListingResponse jobResponse = mapToResponse(job);

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
    public void addJobListingSkill(AddJobListingSkillRequest request,
                                   StreamObserver<JobSkillResponse> responseObserver) {
        try {
            JobListing jobListing = jobListingRepository.findById(request.getJobListingId())
                    .orElseThrow(() -> new RuntimeException("JobListing not found"));

            String skillName = request.getSkillName();
            String category = request.getCategory();

            Skill skill = skillRepository.findByName(skillName)
                    .orElseGet(() -> {
                        Skill newSkill = new Skill(skillName, category);
                        return skillRepository.save(newSkill);
                    });


            JobSkillPriority priority;
            try {
                priority = JobSkillPriority.valueOf(request.getPriority());
            } catch (IllegalArgumentException ex) {
                priority = JobSkillPriority.must;
            }

            JobSkill jobSkill = new JobSkill(jobListing, skill, priority);
            JobSkill saved = jobSkillRepository.save(jobSkill);


            JobSkillResponse response = JobSkillResponse.newBuilder()
                    .setId(saved.getId())
                    .setPriority(saved.getPriority().name())
                    .setJobListingId(jobListing.getId())
                    .setSkillId(skill.getId())
                    .setSkillName(skill.getName())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
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
    public void getJobListingSkills(GetJobListingSkillsRequest request,
                                    StreamObserver<GetJobListingSkillsResponse> responseObserver) {
        try {
            List<JobSkill> jobSkills = jobSkillRepository.findByJobListing_Id(request.getJobListingId());
            List<JobSkillResponse> response = new ArrayList<>();

            for (JobSkill jobSkill : jobSkills) {
                JobSkillResponse jobSkillResponse = JobSkillResponse.newBuilder()
                        .setId(jobSkill.getId())
                        .setPriority(jobSkill.getPriority().name())         // "must"/"nice"
                        .setJobListingId(jobSkill.getJobListing().getId())
                        .setSkillId(jobSkill.getSkill().getId())
                        .setSkillName(jobSkill.getSkill().getName())
                        .build();
                response.add(jobSkillResponse);
            }

            responseObserver.onNext(
                    GetJobListingSkillsResponse.newBuilder()
                            .addAllSkills(response)
                            .build()
            );
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void getJobListingsByCity(GetJobListingsByCityRequest request,
                                     StreamObserver<JobListingsResponse> responseObserver) {
        try {
            List<JobListing> listings = jobListingRepository.findByLocation_CityIgnoreCase(request.getCityName());

            JobListingsResponse.Builder builder =
                    JobListingsResponse.newBuilder();

            for (JobListing job : listings) {
                if (job.isClosed()) {
                    continue;
                }

                JobListingResponse jobResponse = mapToResponse(job);

                builder.addListings(jobResponse);
            }

            responseObserver.onNext(builder.build());
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    private String safe(String val) {
        return val == null ? "" : val;
    }

    private JobListingResponse mapToResponse(JobListing job) {
        return JobListingResponse.newBuilder()
                .setId(job.getId())
                .setTitle(safe(job.getTitle()))
                .setDescription(safe(job.getDescription()))
                .setDatePosted(job.getDatePosted().toString())
                .setSalary(job.getSalary() != null ? job.getSalary().toPlainString() : "")
                .setCompanyId(job.getCompany().getId())
                .setCity(job.getLocation().getCity())
                .setPostcode(job.getLocation().getPostcode() == null ? "" : job.getLocation().getPostcode())
                .setAddress(job.getLocation().getAddress() == null ? "" : job.getLocation().getAddress())
                .setPostedById(job.getPostedBy() != null ? job.getPostedBy().getId() : 0L)
                .setIsClosed(job.isClosed())
                .build();
    }

    @Override
    @Transactional
    public void updateJobListing(UpdateJobListingRequest request,
                                 StreamObserver<JobListingResponse> responseObserver) {
        try {
            JobListing jobListing = jobListingRepository.findById(request.getId())
                    .orElseThrow(() -> new RuntimeException("Job listing not found"));

            jobListing.setTitle(request.getTitle());
            jobListing.setDescription(request.getDescription());

            if (request.getSalary() != null && !request.getSalary().isBlank()) {
                jobListing.setSalary(new BigDecimal(request.getSalary()));
            } else {
                jobListing.setSalary(null);
            }

            Location location = jobListing.getLocation();
            if (location == null) {
                location = new Location(request.getCity(), request.getPostcode(), request.getAddress());
            } else {
                location.setCity(request.getCity());
                location.setPostcode(request.getPostcode());
                location.setAddress(request.getAddress());
            }

            Location savedLocation = locationRepository.save(location);
            jobListing.setLocation(savedLocation);

            if (request.getCompanyId() != 0 && (jobListing.getCompany() == null || !jobListing.getCompany().getId().equals(request.getCompanyId()))) {
                Company company = companyRepository.findById(request.getCompanyId())
                        .orElseThrow(() -> new RuntimeException("Company not found"));
                jobListing.setCompany(company);
            }
            jobListing.setClosed(request.getIsClosed());

            JobListing saved = jobListingRepository.save(jobListing);

            responseObserver.onNext(mapToResponse(saved));
            responseObserver.onCompleted();
        } catch (Exception e) {
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
    public void closeJobListing(CloseJobListingRequest request,
                                StreamObserver<JobListingResponse> responseObserver) {
        try {
            JobListing jobListing = jobListingRepository.findById(request.getId())
                    .orElseThrow(() -> new RuntimeException("Job listing not found"));

            jobListing.setClosed(true);

            JobListing saved = jobListingRepository.save(jobListing);

            responseObserver.onNext(mapToResponse(saved));
            responseObserver.onCompleted();
        } catch (Exception e) {
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
    public void getJobListingById(GetJobListingByIdRequest request,
                                  StreamObserver<JobListingResponse> responseObserver) {
        try {
            JobListing jobListing = jobListingRepository.findById(request.getId())
                    .orElseThrow(() -> new RuntimeException("Job listing not found"));

            responseObserver.onNext(mapToResponse(jobListing));
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(
                    Status.UNKNOWN
                            .withDescription(e.getMessage())
                            .withCause(e)
                            .asRuntimeException()
            );
        }
    }
}

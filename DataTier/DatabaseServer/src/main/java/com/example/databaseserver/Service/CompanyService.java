package com.example.databaseserver.Service;

import com.example.databaseserver.DTOs.UpdateCompanyDto;
import com.example.databaseserver.Entities.Company;
import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Repositories.*;
import com.example.databaseserver.generated.*;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.List;

@GrpcService
public class CompanyService extends CompanyServiceGrpc.CompanyServiceImplBase {

    private final CompanyRepository companyRepository;
    private final LocationRepository locationRepository;
    private final RepresentativeRepository representativeRepository;

    private final RecruiterRepository recruiterRepository;
    private final UserRepository userRepository;
    private final JobListingRepository jobListingRepository;

    @Autowired
    public CompanyService(CompanyRepository companyRepository,
                          LocationRepository locationRepository,
                          RepresentativeRepository representativeRepository,
                          RecruiterRepository recruiterRepository,
                          UserRepository userRepository,
                          JobListingRepository jobListingRepository) {
        this.companyRepository = companyRepository;
        this.locationRepository = locationRepository;
        this.representativeRepository = representativeRepository;
        this.recruiterRepository = recruiterRepository;
        this.userRepository = userRepository;
        this.jobListingRepository = jobListingRepository;
    }


    @Override
    @Transactional
    public void createCompany(CreateCompanyRequest request,
                              StreamObserver<CompanyResponse> responseObserver) {

        try {
            Location location = new Location(
                    request.getCity(),
                    request.getPostcode(),
                    request.getAddress()
            );
            Location savedLocation = locationRepository.save(location);


            CompanyRepresentative representative = representativeRepository
                    .findById(request.getCompanyRepresentativeId())
                    .orElse(null);


            Company company = new Company(
                    request.getName(),
                    request.getDescription(),
                    request.getWebsite(),
                    representative,
                    savedLocation
            );

            Company savedCompany = companyRepository.save(company);

            CompanyResponse response = toCompanyResponse(savedCompany);

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void updateCompany(UpdateCompanyRequest request,
                              StreamObserver<CompanyResponse> responseObserver) {

        try {
            long companyId = request.getCompanyId();

            Company company = companyRepository.findById(companyId)
                    .orElseThrow(() ->
                            new IllegalArgumentException("Company not found with id: " + companyId));

            CompanyRepresentative representative = company.getCompanyRepresentative();
            long requesterRepId = request.getCompanyRepresentativeId();
            if (representative != null && requesterRepId != 0 && representative.getId() != requesterRepId) {
                throw new IllegalArgumentException("You are not allowed to update this company.");
            }

            UpdateCompanyDto dto = new UpdateCompanyDto(
                    companyId,
                    request.getName(),
                    request.getDescription(),
                    request.getWebsite(),
                    request.getCity(),
                    request.getPostcode(),
                    request.getAddress(),
                    requesterRepId
            );

            company.setName(dto.getName());
            company.setDescription(dto.getDescription());
            company.setWebsite(dto.getWebsite());

            Location location = company.getLocation();
            if (location == null) {
                location = new Location();
            }
            location.setCity(dto.getCity());
            location.setPostcode(dto.getPostcode());
            location.setAddress(dto.getAddress());
            location = locationRepository.save(location);

            company.setLocation(location);
            Company savedCompany = companyRepository.save(company);

            responseObserver.onNext(toCompanyResponse(savedCompany));
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void getCompanyById(
            GetCompanyByIdRequest request,
            StreamObserver<CompanyResponse> responseObserver
    ) {
        try {
            long companyId = request.getCompanyId();

            var companyOpt = companyRepository.findById(companyId);
            if (companyOpt.isEmpty()) {
                responseObserver.onError(
                        Status.NOT_FOUND
                                .withDescription("Company with id " + companyId + " not found")
                                .asRuntimeException()
                );
                return;
            }

            Company company = companyOpt.get();

            CompanyResponse response = CompanyResponse.newBuilder()
                    .setId(company.getId())
                    .setName(company.getName())
                    .setDescription(company.getDescription() == null ? "" : company.getDescription())
                    .setWebsite(company.getWebsite() == null ? "" : company.getWebsite())
                    .setIsApproved(company.isApproved())
                    .setCity(company.getLocation().getCity())
                    .setPostcode(company.getLocation().getPostcode())
                    .setAddress(company.getLocation().getAddress())
                    .setCompanyRepresentativeId(company.getCompanyRepresentative().getId())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            responseObserver.onError(
                    Status.INTERNAL
                            .withDescription(e.getMessage())
                            .withCause(e)
                            .asRuntimeException()
            );
        }
    }


    @Override
    @Transactional
    public void getCompaniesForRepresentative(
            GetCompaniesForRepresentativeRequest request,
            StreamObserver<GetCompaniesForRepresentativeResponse> responseObserver) {

        try {
            long repId = request.getCompanyRepresentativeId();

            List<Company> companies =
                    companyRepository.findByCompanyRepresentative_Id(repId);

            GetCompaniesForRepresentativeResponse.Builder responseBuilder =
                    GetCompaniesForRepresentativeResponse.newBuilder();

            for (Company company : companies) {
                responseBuilder.addCompanies(toCompanyResponse(company));
            }

            responseObserver.onNext(responseBuilder.build());
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void getCompaniesToApprove(
            GetCompaniesToApproveRequest request,
            StreamObserver<GetCompaniesToApproveResponse> responseObserver) {

        try {
            List<Company> companies = companyRepository.findByApprovedFalse();

            GetCompaniesToApproveResponse.Builder responseBuilder =
                    GetCompaniesToApproveResponse.newBuilder();

            for (Company company : companies) {
                responseBuilder.addCompanies(toCompanyResponse(company));
            }

            responseObserver.onNext(responseBuilder.build());
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    @Override
    @Transactional
    public void approveCompany(
            ApproveCompanyRequest request,
            StreamObserver<CompanyResponse> responseObserver) {

        try {
            long companyId = request.getCompanyId();

            Company company = companyRepository.findById(companyId)
                    .orElseThrow(() ->
                            new IllegalArgumentException("Company not found with id: " + companyId));

            if (!company.isApproved()) {
                company.setApproved(true);
                company = companyRepository.save(company);
            }

            CompanyResponse response = toCompanyResponse(company);

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    private CompanyResponse toCompanyResponse(Company company) {
        Location loc = company.getLocation();
        CompanyRepresentative rep = company.getCompanyRepresentative();

        return CompanyResponse.newBuilder()
                .setId(company.getId())
                .setName(safe(company.getName()))
                .setDescription(safe(company.getDescription()))
                .setWebsite(safe(company.getWebsite()))
                .setIsApproved(company.isApproved())
                .setCity(loc != null ? safe(loc.getCity()) : "")
                .setPostcode(loc != null ? safe(loc.getPostcode()) : "")
                .setAddress(loc != null ? safe(loc.getAddress()) : "")
                .setCompanyRepresentativeId(rep != null ? rep.getId() : 0L)
                .build();
    }
    @Override
    @Transactional
    public void removeCompany(RemoveCompanyRequest request,
                              StreamObserver<RemoveCompanyResponse> responseObserver) {

        long companyId = request.getId();

        try {
            boolean exists = companyRepository.existsById(companyId);
            if (!exists) {
                RemoveCompanyResponse response = RemoveCompanyResponse.newBuilder()
                        .setSuccess(false)
                        .setMessage("Company not found with id: " + companyId)
                        .build();

                responseObserver.onNext(response);
                responseObserver.onCompleted();
                return;
            }
            List<Long> recruiterUserIds = recruiterRepository.findRecruiterUserIdsByCompanyId(companyId);

            recruiterRepository.deleteAllByCompanyId(companyId);

            if (recruiterUserIds != null && !recruiterUserIds.isEmpty()) {
                userRepository.deleteAllById(recruiterUserIds);
            }

            companyRepository.deleteById(companyId);

            RemoveCompanyResponse response = RemoveCompanyResponse.newBuilder()
                    .setSuccess(true)
                    .setMessage("Company deleted successfully")
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
            RemoveCompanyResponse response = RemoveCompanyResponse.newBuilder()
                    .setSuccess(false)
                    .setMessage("Error deleting company: " + e.getMessage())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
    }


    private String safe(String value) {
        return value == null ? "" : value;
    }
}

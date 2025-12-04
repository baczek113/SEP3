package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.Company;
import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Repositories.CompanyRepository;
import com.example.databaseserver.Repositories.LocationRepository;
import com.example.databaseserver.Repositories.RepresentativeRepository;
import com.example.databaseserver.generated.ApproveCompanyRequest;
import com.example.databaseserver.generated.CompanyResponse;
import com.example.databaseserver.generated.CompanyServiceGrpc;
import com.example.databaseserver.generated.CreateCompanyRequest;
import com.example.databaseserver.generated.GetCompaniesForRepresentativeRequest;
import com.example.databaseserver.generated.GetCompaniesForRepresentativeResponse;
import com.example.databaseserver.generated.GetCompaniesToApproveRequest;
import com.example.databaseserver.generated.GetCompaniesToApproveResponse;
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

    @Autowired
    public CompanyService(CompanyRepository companyRepository,
                          LocationRepository locationRepository,
                          RepresentativeRepository representativeRepository) {
        this.companyRepository = companyRepository;
        this.locationRepository = locationRepository;
        this.representativeRepository = representativeRepository;
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

    // === HELPER: mapowanie encji JPA -> protobuf ===
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

    private String safe(String value) {
        return value == null ? "" : value;
    }
}

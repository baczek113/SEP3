package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.Company;
import com.example.databaseserver.Entities.CompanyRepresentative;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Repositories.CompanyRepository;
import com.example.databaseserver.Repositories.LocationRepository;
import com.example.databaseserver.Repositories.RepresentativeRepository;
import com.example.databaseserver.generated.CompanyResponse;
import com.example.databaseserver.generated.CompanyServiceGrpc;
import com.example.databaseserver.generated.CreateCompanyRequest;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;

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


            CompanyResponse.Builder builder = CompanyResponse.newBuilder()
                    .setId(savedCompany.getId())
                    .setName(savedCompany.getName())
                    .setDescription(savedCompany.getDescription() == null ? "" : savedCompany.getDescription())
                    .setWebsite(savedCompany.getWebsite() == null ? "" : savedCompany.getWebsite())
                    .setIsApproved(savedCompany.isApproved());

            if (savedCompany.getLocation() != null) {
                builder.setCity(savedCompany.getLocation().getCity() == null ? "" : savedCompany.getLocation().getCity())
                        .setPostcode(savedCompany.getLocation().getPostcode() == null ? "" : savedCompany.getLocation().getPostcode())
                        .setAddress(savedCompany.getLocation().getAddress() == null ? "" : savedCompany.getLocation().getAddress());
            }

            if (savedCompany.getCompanyRepresentative() != null) {
                builder.setCompanyRepresentativeId(savedCompany.getCompanyRepresentative().getId());
            } else {
                builder.setCompanyRepresentativeId(0L);
            }

            CompanyResponse response = builder.build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }
}

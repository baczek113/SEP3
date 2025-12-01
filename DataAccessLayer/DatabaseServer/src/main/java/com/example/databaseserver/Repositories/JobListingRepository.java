package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.JobListing;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface JobListingRepository extends JpaRepository<JobListing, Long> {

    List<JobListing> findByCompany_Id(Long companyId);

    List<JobListing> findByPostedBy_Id(Long recruiterId);

    List<JobListing> findByLocation_CityIgnoreCase(String city);
}

package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Company;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface CompanyRepository extends JpaRepository<Company, Long> {

    List<Company> findByCompanyRepresentative_Id(Long representativeId);
}

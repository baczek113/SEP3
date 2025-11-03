package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.CompanyRepresentative;
import org.springframework.data.jpa.repository.JpaRepository;

public interface RepresentativeRepository extends JpaRepository<CompanyRepresentative, Long>{
}

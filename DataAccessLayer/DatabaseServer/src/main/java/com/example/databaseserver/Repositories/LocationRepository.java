package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Location;
import org.springframework.data.jpa.repository.JpaRepository;

public interface LocationRepository extends JpaRepository<Location, Long> {
}

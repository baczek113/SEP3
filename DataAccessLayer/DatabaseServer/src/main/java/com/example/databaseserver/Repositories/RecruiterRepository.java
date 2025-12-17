package com.example.databaseserver.Repositories;

import com.example.databaseserver.Entities.Recruiter;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface RecruiterRepository extends JpaRepository<Recruiter, Long> {

    List<Recruiter> findByWorksIn_Id(Long companyId);

    List<Recruiter> findByHiredBy_Id(Long representativeId);

    @Query(value = """
        SELECT u.id
        FROM recruiter r
        JOIN company c ON r.works_in = c.id
        JOIN company_representative cr ON c.cr_id = cr.user_id
        JOIN users u ON r.user_id = u.id
        WHERE cr.user_id = :repId
        """, nativeQuery = true)
    List<Long> findRecruiterUserIdsByRepresentativeId(@Param("repId") Long representativeId);

    @Query(value = """
    SELECT r.user_id
    FROM recruiter r
    WHERE r.works_in = :companyId
    """, nativeQuery = true)
    List<Long> findRecruiterUserIdsByCompanyId(@Param("companyId") Long companyId);

    @Modifying
    @Query(value = """
    DELETE FROM recruiter
    WHERE works_in = :companyId
    """, nativeQuery = true)
    void deleteAllByCompanyId(@Param("companyId") Long companyId);

}


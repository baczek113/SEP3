package com.example.databaseserver.Entities;

import org.junit.jupiter.api.Test;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;

import static org.junit.jupiter.api.Assertions.*;

public class JobListingTest {

    // -----------------------------------
    // ID tests
    // -----------------------------------

    @Test
    void testInitialIdIsNull() {
        JobListing jobListing = createBasicJobListing();
        assertNull(jobListing.getId());
    }

    @Test
    void testSetId() {
        JobListing jobListing = createBasicJobListing();
        jobListing.setId(1L);
        assertEquals(1L, jobListing.getId());
    }

    // -----------------------------------
    // Title tests
    // -----------------------------------

    @Test
    void testTitleIsStoredCorrectly() {
        JobListing jobListing = new JobListing(
                "Junior Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals("Junior Java Developer", jobListing.getTitle());
    }

    @Test
    void testTitleBoundaryLength() {
        String longTitle = "A".repeat(255);
        JobListing jobListing = new JobListing(
                longTitle,
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals(longTitle, jobListing.getTitle());
    }

    @Test
    void testEmptyTitle() {
        JobListing jobListing = new JobListing(
                "",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals("", jobListing.getTitle());
    }

    // -----------------------------------
    // Description tests
    // -----------------------------------

    @Test
    void testNullDescription() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                null,
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertNull(jobListing.getDescription());
    }

    @Test
    void testDescriptionIsStoredCorrectly() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "We are looking for a Java developer.",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals("We are looking for a Java developer.", jobListing.getDescription());
    }

    // -----------------------------------
    // datePosted tests
    // -----------------------------------

    @Test
    void testDatePostedIsStoredCorrectly() {
        OffsetDateTime dateTime = OffsetDateTime.of(2024, 10, 10, 10, 10, 0, 0, ZoneOffset.UTC);
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                dateTime,
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals(dateTime, jobListing.getDatePosted());
    }

    @Test
    void testNullDatePosted() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                null,
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertNull(jobListing.getDatePosted());
    }

    // -----------------------------------
    // Salary tests
    // -----------------------------------

    @Test
    void testSalaryIsStoredCorrectly() {
        BigDecimal salary = new BigDecimal("5000.00");
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                salary,
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals(salary, jobListing.getSalary());
    }

    @Test
    void testZeroSalary() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                BigDecimal.ZERO,
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertEquals(BigDecimal.ZERO, jobListing.getSalary());
    }

    @Test
    void testNullSalary() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                null,
                new Company(),
                new Location(),
                new Recruiter()
        );
        assertNull(jobListing.getSalary());
    }

    // -----------------------------------
    // Company tests
    // -----------------------------------

    @Test
    void testCompanyIsStoredCorrectly() {
        Company company = new Company();
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                company,
                new Location(),
                new Recruiter()
        );
        assertEquals(company, jobListing.getCompany());
    }

    @Test
    void testNullCompany() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                null,
                new Location(),
                new Recruiter()
        );
        assertNull(jobListing.getCompany());
    }

    // -----------------------------------
    // Location tests
    // -----------------------------------

    @Test
    void testLocationIsStoredCorrectly() {
        Location location = new Location();
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                location,
                new Recruiter()
        );
        assertEquals(location, jobListing.getLocation());
    }

    @Test
    void testNullLocation() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                null,
                new Recruiter()
        );
        assertNull(jobListing.getLocation());
    }

    // -----------------------------------
    // postedBy tests
    // -----------------------------------

    @Test
    void testPostedByIsStoredCorrectly() {
        Recruiter recruiter = new Recruiter();
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                recruiter
        );
        assertEquals(recruiter, jobListing.getPostedBy());
    }

    @Test
    void testNullPostedBy() {
        JobListing jobListing = new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                null
        );
        assertNull(jobListing.getPostedBy());
    }

    // -----------------------------------
    // Helpers
    // -----------------------------------

    private JobListing createBasicJobListing() {
        return new JobListing(
                "Java Developer",
                "Description",
                sampleDateTime(),
                new BigDecimal("5000.00"),
                new Company(),
                new Location(),
                new Recruiter()
        );
    }

    private OffsetDateTime sampleDateTime() {
        return OffsetDateTime.of(2024, 10, 10, 10, 10, 0, 0, ZoneOffset.UTC);
    }
}

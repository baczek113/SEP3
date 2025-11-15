package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.*;
import com.example.databaseserver.Entities.Applicant;
import com.example.databaseserver.Repositories.ApplicantRepository;
import com.example.databaseserver.Repositories.ApplicantSkillRepository;
import com.example.databaseserver.Repositories.LocationRepository;
import com.example.databaseserver.Repositories.SkillRepository;
import com.example.databaseserver.generated.*;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;

@GrpcService
public class ApplicantService extends ApplicantServiceGrpc.ApplicantServiceImplBase {

    private final ApplicantRepository applicantRepository;
    private final LocationRepository locationRepository;
    private final SkillRepository skillRepository;
    private final ApplicantSkillRepository applicantSkillRepository;

    @Autowired
    public ApplicantService(ApplicantRepository applicantRepository,
                            LocationRepository locationRepository, ApplicantSkillRepository applicantSkillRepository, SkillRepository skillRepository) {
        this.applicantRepository = applicantRepository;
        this.locationRepository = locationRepository;
        this.applicantSkillRepository = applicantSkillRepository;
        this.skillRepository = skillRepository;
    }

    @Override
    @Transactional
    public void createApplicant(CreateApplicantRequest request,
                                StreamObserver<ApplicantResponse> responseObserver) {
        try {
            User user = new User(
                    request.getEmail(),
                    request.getPasswordHash(),
                    UserRole.applicant,
                    request.getName()
            );

            Location location = new Location(
                    request.getCity(),
                    request.getPostcode(),
                    request.getAddress()
            );
            Location savedLocation = locationRepository.save(location);

            Applicant applicant = new Applicant(
                    user,
                    request.getExperience(),
                    savedLocation
            );

            Applicant savedApplicant = applicantRepository.save(applicant);

            ApplicantResponse response = ApplicantResponse.newBuilder()
                    .setId(savedApplicant.getId())
                    .setName(savedApplicant.getUser().getName())
                    .setEmail(savedApplicant.getUser().getEmail())
                    .setExperience(
                            savedApplicant.getExperience() == null
                                    ? ""
                                    : savedApplicant.getExperience()
                    )
                    .setCity(savedApplicant.getLocation().getCity())
                    .setPostcode(
                            savedApplicant.getLocation().getPostcode() == null
                                    ? ""
                                    : savedApplicant.getLocation().getPostcode()
                    )
                    .setAddress(
                            savedApplicant.getLocation().getAddress() == null
                                    ? ""
                                    : savedApplicant.getLocation().getAddress()
                    )
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(e);
        }
    }
    @Override
    @Transactional
    public void addApplicantSkill(AddApplicantSkillRequest request,
                                  StreamObserver<ApplicantSkillResponse> responseObserver) {
        try {
            Applicant applicant = applicantRepository.findById(request.getApplicantId())
                    .orElseThrow(() -> new RuntimeException(
                            "Applicant not found with id: " + request.getApplicantId()
                    ));

            Skill skill = skillRepository.findByName(request.getSkillName())
                    .orElseGet(() -> {
                        String category = request.getCategory().isBlank()
                                ? null
                                : request.getCategory();
                        Skill newSkill = new Skill(request.getSkillName(), category);
                        return skillRepository.save(newSkill);
                    });

            boolean exists = applicantSkillRepository
                    .existsByApplicant_IdAndSkill_Id(applicant.getId(), skill.getId());

            if (exists) {
                throw new RuntimeException("Applicant already has this skill assigned.");
            }

            SkillLevel level = mapProtoToEntityLevel(request.getLevel());

            ApplicantSkill applicantSkill = new ApplicantSkill(applicant, skill, level);
            ApplicantSkill saved = applicantSkillRepository.save(applicantSkill);

            // 6) Zbuduj odpowiedź dla klienta gRPC
            ApplicantSkillResponse response = ApplicantSkillResponse.newBuilder()
                    .setId(saved.getId())
                    .setApplicantId(applicant.getId())
                    .setSkillId(skill.getId())
                    .setSkillName(skill.getName())
                    .setLevel(mapEntityToProtoLevel(saved.getLevel()))
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            responseObserver.onError(e);
        }
    }

    private SkillLevel mapProtoToEntityLevel(SkillLevelProto protoLevel) {
        return switch (protoLevel) {
            case SKILL_LEVEL_BEGINNER -> SkillLevel.beginner;
            case SKILL_LEVEL_JUNIOR   -> SkillLevel.junior;
            case SKILL_LEVEL_MID      -> SkillLevel.mid;
            case SKILL_LEVEL_SENIOR   -> SkillLevel.senior;
            case SKILL_LEVEL_EXPERT   -> SkillLevel.expert;
            case UNRECOGNIZED, SKILL_LEVEL_UNSPECIFIED -> SkillLevel.beginner;
        };
    }

    private SkillLevelProto mapEntityToProtoLevel(SkillLevel level) {
        return switch (level) {
            case beginner -> SkillLevelProto.SKILL_LEVEL_BEGINNER;
            case junior   -> SkillLevelProto.SKILL_LEVEL_JUNIOR;
            case mid      -> SkillLevelProto.SKILL_LEVEL_MID;
            case senior   -> SkillLevelProto.SKILL_LEVEL_SENIOR;
            case expert   -> SkillLevelProto.SKILL_LEVEL_EXPERT;
        };
    }


}

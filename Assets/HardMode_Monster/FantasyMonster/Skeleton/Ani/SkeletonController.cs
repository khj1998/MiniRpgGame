using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    public float detectionRadius = 5f; // 플레이어 탐지 범위
    public float walkSpeed = 2f; // 걷는 속도
    public float runSpeed = 5f; // 뛰는 속도
    private Animator animator;
    private Transform player;
    private bool isChasingPlayer = false; // 플레이어를 추적 중인지 여부
    private float stateTimer = 0f; // 상태 전환 타이머
    private bool isRunning = false; // 현재 Run 상태인지 여부

    void Start()
    {
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        // Player 태그를 가진 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!isChasingPlayer)
        {
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 탐지 범위 내에 있을 경우 Walk 상태로 전환
            if (distanceToPlayer <= detectionRadius)
            {
                animator.SetBool("IsWalking", true);
                isChasingPlayer = true; // 추적 시작
            }
        }

        if (isChasingPlayer)
        {
            UpdateMovementState(); // 상태 관리
            MoveTowardsPlayer(); // 이동
        }
    }

    void UpdateMovementState()
    {
        stateTimer += Time.deltaTime;

        if (isRunning && stateTimer >= 3f)
        {
            // Run 상태가 3초 지속되면 Walk로 전환
            isRunning = false;
            stateTimer = 0f;
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", true);
        }
        else if (!isRunning && stateTimer >= 5f)
        {
            // Walk 상태가 5초 지속되면 Run으로 전환
            isRunning = true;
            stateTimer = 0f;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
        }
    }

    void MoveTowardsPlayer()
    {
        // 플레이어 방향 계산
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Skeleton이 플레이어를 바라보도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 부드럽게 회전

        // 현재 속도에 따라 이동
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.MovePosition(transform.position + directionToPlayer * currentSpeed * Time.deltaTime);
        }
        else
        {
            // Rigidbody가 없는 경우 Transform으로 이동
            transform.position += directionToPlayer * currentSpeed * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 탐지 범위를 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
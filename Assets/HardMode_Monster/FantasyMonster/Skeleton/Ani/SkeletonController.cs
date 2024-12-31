using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    public float detectionRadius = 5f; // �÷��̾� Ž�� ����
    public float walkSpeed = 2f; // �ȴ� �ӵ�
    public float runSpeed = 5f; // �ٴ� �ӵ�
    private Animator animator;
    private Transform player;
    private bool isChasingPlayer = false; // �÷��̾ ���� ������ ����
    private float stateTimer = 0f; // ���� ��ȯ Ÿ�̸�
    private bool isRunning = false; // ���� Run �������� ����

    void Start()
    {
        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        // Player �±׸� ���� ������Ʈ ã��
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!isChasingPlayer)
        {
            // �÷��̾���� �Ÿ� ���
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Ž�� ���� ���� ���� ��� Walk ���·� ��ȯ
            if (distanceToPlayer <= detectionRadius)
            {
                animator.SetBool("IsWalking", true);
                isChasingPlayer = true; // ���� ����
            }
        }

        if (isChasingPlayer)
        {
            UpdateMovementState(); // ���� ����
            MoveTowardsPlayer(); // �̵�
        }
    }

    void UpdateMovementState()
    {
        stateTimer += Time.deltaTime;

        if (isRunning && stateTimer >= 3f)
        {
            // Run ���°� 3�� ���ӵǸ� Walk�� ��ȯ
            isRunning = false;
            stateTimer = 0f;
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", true);
        }
        else if (!isRunning && stateTimer >= 5f)
        {
            // Walk ���°� 5�� ���ӵǸ� Run���� ��ȯ
            isRunning = true;
            stateTimer = 0f;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
        }
    }

    void MoveTowardsPlayer()
    {
        // �÷��̾� ���� ���
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Skeleton�� �÷��̾ �ٶ󺸵��� ȸ��
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // �ε巴�� ȸ��

        // ���� �ӵ��� ���� �̵�
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.MovePosition(transform.position + directionToPlayer * currentSpeed * Time.deltaTime);
        }
        else
        {
            // Rigidbody�� ���� ��� Transform���� �̵�
            transform.position += directionToPlayer * currentSpeed * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Ž�� ������ �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
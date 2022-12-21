using System.Collections;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    [SerializeField]
    private EnemyScriptable enemyParams;
    private float speed;
    private int damage;
    private float attackSpeed;
    private float bulletSpeed;
    private float distance;
    private int health;
    private float range;
    private float minDistance;
    private bool facingRight;
    private SpriteRenderer spriteRend;
    private Rigidbody2D rigid;
    private float movementSmoothing = 0.05f;
    private Vector3 velocity = Vector3.zero;
    private bool canAttack;
    private Animator animator;
    private Transform beak;
    [SerializeField] 
    LayerMask mask;
    // Start is called before the first frame update
    void Awake() {
        speed = enemyParams.speed;
        damage = enemyParams.damage;
        attackSpeed = enemyParams.attackSpeed;
        minDistance = enemyParams.minDistance;
        health = enemyParams.health;
        range = enemyParams.range;
        facingRight = false;
        canAttack = true;
        animator = gameObject.GetComponent<Animator>();
        beak = gameObject.GetComponentInChildren<Transform>();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spriteRend = this.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // Calculate the distance to the player
        distance = Vector2.Distance(transform.position, player.transform.position);
        // Calculate position difference to set object's velocity
        Vector2 dif = (player.transform.position - transform.position).normalized * speed;

        if(distance >= minDistance) {
            // Let the enemy move, but not rotate. Treat it as an obstacle
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.velocity = Vector3.SmoothDamp(rigid.velocity, dif, ref velocity, movementSmoothing);
        }
        // If the enemy should escape the player add this
        /*else if(distance < minDistance) {
            rigid.constraints = RigidbodyConstraints2D.None;
            dif *= -(1/0.1f * distance);
            
        }*/
        else {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            rigid.velocity = Vector3.zero;
        }

        rigid.velocity = Vector3.SmoothDamp(rigid.velocity, dif, ref velocity, movementSmoothing);

        // Flip if there is a need
        if(transform.position.x < player.transform.position.x && !facingRight) Flip();
        if(transform.position.x > player.transform.position.x && facingRight) Flip();

        // Check if attack makes any sense (player in range)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position, range, mask);
        if(Physics2D.Raycast(transform.position, player.transform.position, range, mask)) Debug.Log(hit.transform.tag);
        /*if(hit.transform.tag == "Player" && canAttack) { // Player in range
            // If so perform an attack
            PerformAttack();
        }*/
    }

    private void Flip()
	{
        facingRight = !facingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public void TakeDamage(int damage) {
        health -= damage;
        if(health <= 0) Destroy(this.gameObject);
    }

    private IEnumerator ShootDelay() {
        yield return new WaitForSeconds(attackSpeed);
        // Do not shoot if you're reloading
        canAttack = true;
    }

    void PerformAttack() {
        animator.Play("EnemyAttack");
        canAttack = false;
        // Same as gun - instantiate a bullet + shoot it at player's position
        string path = "Assets/Prefabs/Bullet.prefab";
        GameObject bullet = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        bullet = GameObject.Instantiate(bullet, beak.position, Quaternion.identity) as GameObject;
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        // Calcualte bullet's direction
        Vector2 direction = (player.transform.position - new Vector3(beak.position.x, beak.position.y)).normalized;
        direction = direction * bulletSpeed * Time.fixedDeltaTime;

        Debug.Log("XD");
        bulletScript.SetParams(direction, damage);
            
        StartCoroutine(ShootDelay());
    }
}
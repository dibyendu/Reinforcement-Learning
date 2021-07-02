using UnityEngine;

public class HitWall : MonoBehaviour
{
    public GameObject areaObject;
    public int lastAgentHit;
    public bool net;

    public enum FloorHit
        {
            Service,
            FloorHitUnset,
            FloorAHit,
            FloorBHit
        }

    public FloorHit lastFloorHit;

    TennisArea m_Area;
    TennisAgent m_AgentA;
    TennisAgent m_AgentB;

    //  Use this for initialization
    void Start()
    {
        m_Area = areaObject.GetComponent<TennisArea>();
        m_AgentA = m_Area.agentA.GetComponent<TennisAgent>();
        m_AgentB = m_Area.agentB.GetComponent<TennisAgent>();
    }

    void Reset()
    {
        m_AgentA.EndEpisode();
        m_AgentB.EndEpisode();
        m_Area.MatchReset();
        lastFloorHit = FloorHit.Service;
        net = false;
    }
    
    void AgentAWins()
    {
        m_AgentA.score += 1;
        Reset();
    }

    void AgentBWins()
    {
        m_AgentB.score += 1;
        Reset();

    }

    // Rewards
    public float WALL_HIT          = -0.01f, // for hitting into wall
                 FLOOR_HIT         = -0.01f, // for hiting into floor, double bounce or missing service
                 NET_SERVE         = -0.01f, // for serving into net
                 DOUBLE_HIT        = -0.01f, // for double hitting

                 BALL_MISS         = -0.50f, // for missing the ball
                 LONG_HIT          = -0.05f, // for hitting long

                 OVER_NET          = +0.10f, // for sending the ball over the net
                 RESIST_TEMPTATION = +0.50f; // for controlling the temptation of blocking long shot

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("iWall"))
        {
            if (collision.gameObject.name == "wallA")
            {
                // Agent A hits into wall or agent B hits a winner
                if (lastAgentHit == 0 || lastFloorHit == FloorHit.FloorAHit)
                {
                    // Agent A hits into wall
                    if (lastAgentHit == 0) {
                        m_AgentA.AddReward(WALL_HIT);
                    }
                    // Agent A misses the ball
                    else {
                        m_AgentA.AddReward(BALL_MISS);
                    }
                    AgentBWins();
                }
                // Agent B hits long
                else
                {
                    m_AgentA.AddReward(RESIST_TEMPTATION); // for controlling the temptation
                    m_AgentB.AddReward(LONG_HIT);
                    AgentAWins();
                }
            }
            else if (collision.gameObject.name == "wallB")
            {
                // Agent B hits into wall or agent A hits a winner
                if (lastAgentHit == 1 || lastFloorHit == FloorHit.FloorBHit)
                {
                    // Agent B hits into wall
                    if (lastAgentHit == 1) {
                        m_AgentB.AddReward(WALL_HIT);
                    }
                    // Agent B misses the ball
                    else {
                        m_AgentB.AddReward(BALL_MISS);
                    }
                    AgentAWins();
                }
                // Agent A hits long
                else
                {
                    m_AgentA.AddReward(LONG_HIT);
                    m_AgentB.AddReward(RESIST_TEMPTATION); // for controlling the temptation
                    AgentBWins();
                }
            }
            else if (collision.gameObject.name == "floorA")
            {
                // Agent A hits into floor, double bounce or misses service
                if (lastAgentHit == 0 || lastFloorHit == FloorHit.FloorAHit || lastFloorHit == FloorHit.Service)
                {
                    m_AgentA.AddReward(FLOOR_HIT);
                    AgentBWins();
                }
                else
                {
                    lastFloorHit = FloorHit.FloorAHit;
                    // Successful serve by Agent B
                    if (!net)
                    {
                        net = true;
                    }
                    // Agent B successfully sends the ball over the net during a game
                    else
                    {
                        
                    }
                    m_AgentB.AddReward(OVER_NET);
                }
            }
            else if (collision.gameObject.name == "floorB")
            {
                // Agent B hits into floor, double bounce or misses service
                if (lastAgentHit == 1 || lastFloorHit == FloorHit.FloorBHit || lastFloorHit == FloorHit.Service)
                {
                    m_AgentB.AddReward(FLOOR_HIT);
                    AgentAWins();
                }
                else
                {
                    lastFloorHit = FloorHit.FloorBHit;
                    // Successful serve by Agent A
                    if (!net)
                    {
                        net = true;
                    }
                    // Agent A successfully sends the ball over the net during a game
                    else
                    {

                    }
                    m_AgentA.AddReward(OVER_NET);
                }
            }
            else if (collision.gameObject.name == "net" && !net)
            {
                // Agent A serves into net
                if (lastAgentHit == 0)
                {
                    m_AgentA.AddReward(NET_SERVE);
                    AgentBWins();
                }
                // Agent B serves into net
                else if (lastAgentHit == 1)
                {
                    m_AgentB.AddReward(NET_SERVE);
                    AgentAWins();
                }
            }
        }
        else if (collision.gameObject.name == "AgentA")
        {
            // Agent A double hits
            if (lastAgentHit == 0)
            {
                m_AgentA.AddReward(DOUBLE_HIT);
                AgentBWins();
            }
            else
            {
                if (lastFloorHit != FloorHit.Service)
                {
                    if (!net) {
                        net = true;
                    }
                    // Agent A receives the ball straight from Agent B
                    if (lastFloorHit == FloorHit.FloorHitUnset)
                    {
                        m_AgentB.AddReward(OVER_NET);
                    }
                    // Agent A receives the ball after being bounced from its own floor
                    else
                    {

                    }
                }
                lastAgentHit = 0;
                lastFloorHit = FloorHit.FloorHitUnset;
            }
        }
        else if (collision.gameObject.name == "AgentB")
        {
            // Agent B double hits
            if (lastAgentHit == 1)
            {
                m_AgentB.AddReward(DOUBLE_HIT);
                AgentAWins();
            }
            else
            {
                if (lastFloorHit != FloorHit.Service)
                {
                    if (!net) {
                        net = true;
                    }
                    // Agent B receives the ball straight from Agent A
                    if (lastFloorHit == FloorHit.FloorHitUnset)
                    {
                        m_AgentA.AddReward(OVER_NET);
                    }
                    // Agent B receives the ball after being bounced from its own floor
                    else
                    {

                    }
                }
                lastAgentHit = 1;
                lastFloorHit = FloorHit.FloorHitUnset;
            }
        }
    }
}
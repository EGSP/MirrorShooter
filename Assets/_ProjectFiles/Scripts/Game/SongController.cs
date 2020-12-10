using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [Header("Change_of_Weapon")]
    [SerializeField] private AudioClip[] Change_of_Weapon;
    [Header("Run")]
    [SerializeField] private AudioClip[] Footsteps_Human_Run;
    [Header("Walk")]
    [SerializeField] private AudioClip[] Footsteps_Human_Walk;
    [Header("Impact_Body_10_Versions")]
    [SerializeField] private AudioClip[] Impact_Body_10_Versions;
    [Header("Impacts_Concrete_3_Calibers")]
    [SerializeField] private AudioClip[] Impacts_Concrete_3_Calibers;
    [Header("Impacts_Metal_3_Calibers")]
    [SerializeField] private AudioClip[] Impacts_Metal_3_Calibers;
    [Header("Jump_Height_Versions")]
    [SerializeField] private AudioClip[] Jump_Height_Versions;
    [Header("Jump_Length_Versions")]
    [SerializeField] private AudioClip[] Jump_Length_Versions;

    [Header("Weapon_ACR")]
    [SerializeField] private AudioClip ACR_trigger;
    [SerializeField] private AudioClip ACR_single_last_shot;
    [SerializeField] private AudioClip ACR_shell_drop;
    [SerializeField] private AudioClip ACR_reload;
    [SerializeField] private AudioClip ACR_burst_without_last_shot;
    [SerializeField] private AudioClip ACR_burst_with_last_shot;

    [Header("Weapon_Barret")]
    [SerializeField] private AudioClip barret_burst_with_last_shot;
    [SerializeField] private AudioClip barret_reload;
    [SerializeField] private AudioClip barret_shell_drop;
    [SerializeField] private AudioClip barret_shot_with_no_tail;
    [SerializeField] private AudioClip barret_single_shot_with_tail;
    [SerializeField] private AudioClip barret_trigger;

    [Header("Weapon_Handgrenade")]
    [SerializeField] private AudioClip grenade_explosion;
    [SerializeField] private AudioClip grenade_pin;
    [SerializeField] private AudioClip grenade_rolling_on_surface;
    [SerializeField] private AudioClip grenade_whoosh;

    [Header("Weapon_Knife_Stabs_on_Air_Surface_Body")]
    [SerializeField] private AudioClip knife_air_stab_1;
    [SerializeField] private AudioClip knife_air_stab_2;
    [SerializeField] private AudioClip knife_air_stab_3;
    [SerializeField] private AudioClip knife_body_stab_and_taking_out;
    [SerializeField] private AudioClip knife_surface_stab;
    [SerializeField] private AudioClip knife_unsheathing_and_whoosh_for_all_stabs;

    [Header("Weapon_SPAS")]
    [SerializeField] private AudioClip weapon_SPAS_reload;
    [SerializeField] private AudioClip weapon_SPAS_reload_1;
    [SerializeField] private AudioClip weapon_SPAS_shell_drop;
    [SerializeField] private AudioClip weapon_SPAS_shell_out;
    [SerializeField] private AudioClip weapon_SPAS_shot;

    [Header("Weapons_Grenade_Launcher")]
    [SerializeField] private AudioClip grenade_launcher_impact_hit;
    [SerializeField] private AudioClip grenade_launcher_reload;
    [SerializeField] private AudioClip grenade_launcher_shell_drop;
    [SerializeField] private AudioClip grenade_launcher_shot;
    [SerializeField] private AudioClip grenade_launcher_trigger;

    void Start()
    {
        
    }

    public void PlaySong(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}

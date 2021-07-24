using System.Collections;
using UnityEditor;
using UnityEngine;

public static class SoundSFX
{
	private static readonly DictEnumKeyGenericVal<string> dicString;

	static SoundSFX()
	{
		dicString = new DictEnumKeyGenericVal<string>();
		dicString.Add(0, "dg_sfx_block_combo_1");
		dicString.Add(1, "dg_sfx_block_combo_2");
		dicString.Add(2, "dg_sfx_block_combo_3");
		dicString.Add(3, "dg_sfx_block_combo_4");
		dicString.Add(4, "dg_sfx_block_combo_5");
		dicString.Add(5, "dg_sfx_block_combo_6");
		dicString.Add(6, "dg_sfx_block_combo_7");
		dicString.Add(7, "dg_sfx_block_combo_8");
		dicString.Add(8, "SR_ingame_sfx_combo_09");
		dicString.Add(9, "SR_ingame_sfx_combo_10");
		dicString.Add(10, "SR_ingame_sfx_shuffle");
		dicString.Add(12, "SR_ingame_sfx_slide_in");
		dicString.Add(13, "SR_ingame_sfx_slide_out");
		dicString.Add(14, "SR_ingame_sfx_zoom_in");
		dicString.Add(15, "SR_ingame_sfx_zoom_out");
		dicString.Add(16, "SR_ingame_sfx_Star_1");
		dicString.Add(17, "SR_ingame_sfx_Star_2");
		dicString.Add(18, "SR_ingame_sfx_Star_3");
		dicString.Add(19, "SR_ingame_sfx_count_end");
		dicString.Add(20, "SR_ingame_sfx_count_loop");
		dicString.Add(26, "SR_ingame_sfx_SF");
		dicString.Add(28, "SR_ingame_item_mh");
		dicString.Add(30, "SR_ingame_item_cb");
		dicString.Add(31, "SR_ingame_item_hammer");
		dicString.Add(32, "SR_ingame_item_cb_used");
		dicString.Add(59, "SR_ingame_sfx_click");
		dicString.Add(60, "SR_ingame_sfx_move");
		dicString.Add(106, "SR_lobby_sfx_click");
		dicString.Add(108, "SR_lobby_sfx_popup_close");
		dicString.Add(119, "SR_ingame_buy_item");
		dicString.Add(120, "SR_ingame_sfx_5move_Fin");
		dicString.Add(123, "SR_lobby_sfx_popup_high");
		dicString.Add(124, "SR_lobby_sfx_popup_mid");
		dicString.Add(125, "SR_lobby_sfx_popup_low");
		dicString.Add(126, "SR_lobby_sfx_popup_slide");
		dicString.Add(127, "SR_Lobby_sfx_dailybonus_get");
		dicString.Add(41, "SR_ingame_sfx_fusion");
		dicString.Add(23, "SR_ingame_sfx_block_clear");
		dicString.Add(44, "SR_ingame_sfx_15_boom");
		dicString.Add(45, "SR_ingame_sfx_15_firework");
		dicString.Add(24, "SR_ingame_sfx_block_move");
		dicString.Add(25, "SR_ingame_sfx_block_missmatch");
		dicString.Add(42, "SR_ingame_sfx_14_1");
		dicString.Add(43, "SR_ingame_sfx_14_2");
		dicString.Add(52, "SR_ingame_sfx_2222_end");
		dicString.Add(53, "SR_ingame_sfx_2222_loop");
		dicString.Add(54, "SR_ingame_sfx_33");
		dicString.Add(55, "SR_ingame_sfx_3333");
		dicString.Add(56, "SR_ingame_sfx_4133");
		dicString.Add(57, "SR_ingame_sfx_4141");
		dicString.Add(58, "SR_ingame_sfx_5151");
		dicString.Add(49, "SR_ingame_sfx_22_start");
		dicString.Add(50, "SR_ingame_sfx_22_end");
		dicString.Add(51, "SR_ingame_sfx_22_loop");
		dicString.Add(46, "SR_ingame_sfx_15_end");
		dicString.Add(47, "SR_ingame_sfx_15_loop");
		dicString.Add(48, "SR_ingame_sfx_15_start");
		dicString.Add(62, "SR_ingame_sfx_collect");
		dicString.Add(63, "SR_ingame_obj_mission_fly");
		dicString.Add(64, "SR_ingame_obj_mission_get");
		dicString.Add(36, "SR_ingame_obj_ginger_S");
		dicString.Add(37, "SR_ingame_obj_ginger_L");
		dicString.Add(65, "SR_ingame_obj_chocolet");
		dicString.Add(66, "SR_ingame_obj_cracker");
		dicString.Add(72, "SR_ingame_obj_jelly");
		dicString.Add(83, "SR_ingame_obj_diggingblock_s");
		dicString.Add(84, "SR_ingame_obj_diggingblock");
		dicString.Add(85, "SR_ingame_sfx_5move");
		dicString.Add(86, "SR_ingame_sfx_exblock");
		dicString.Add(94, "SR_ingame_bgm_SF");
		dicString.Add(95, "SR_ingame_bgm_SF_loop");
		dicString.Add(96, "SR_ingame_bgm_SF_end");
		dicString.Add(97, "SR_ingame_bgm_mode_FE");
		dicString.Add(98, "SR_ingame_bgm_mode_FE");
		dicString.Add(99, "SR_ingame_bgm_mode_FE");
		dicString.Add(100, "SR_ingame_bgm_mode_FE");
		dicString.Add(101, "SR_ingame_bgm_mode_FE");
		dicString.Add(102, "SR_ingame_bgm_mode_FE");
		dicString.Add(103, "SR_ingame_bgm_mode_FE");
		dicString.Add(104, "SR_ingame_bgm_mode_FE");
		dicString.Add(27, "SR_ingame_bgm_gameover");
		dicString.Add(113, "SR_ingame_buy_coin_start");
		dicString.Add(114, "SR_ingame_buy_coin_end");
	}

	public static AudioSource Play(SFXIndex index, bool loop = false, float delay = 0f)
	{
        if (!dicString.ContainsKey((int)index))
        {
            return null;
        }
        string clipName = dicString[(int)index];

        return SoundManager.PlaySFX(clipName, loop, delay, float.MaxValue, float.MaxValue, default(Vector3), null);
    }

	public static AudioSource PlayCap(SFXIndex index, string cappedId = null)
	{
		if (!dicString.ContainsKey((int)index))
		{
			return null;
		}
		if (string.IsNullOrEmpty(cappedId))
		{
			cappedId = SoundManager.Instance.GetGroupName(dicString[(int)index]);
		}
        return SoundManager.PlayCappedSFX(dicString[(int)index], cappedId);

        return null;
	}

	private static IEnumerator ProcessPlayCapDelay(float delay, SFXIndex index, string cappedId = null)
	{
		yield return new WaitForSeconds(delay);
		PlayCap(index, cappedId);
	}

	public static void PlayCapDelay(MonoBehaviour mono, float delay, SFXIndex index, string cappedId = null)
	{
		mono.StartCoroutine(ProcessPlayCapDelay(delay, index, cappedId));
	}
}

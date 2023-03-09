//--------------------------------------------------------
//    [Author]:               Sausage
//    [  Date ]:             2023年3月9日
//--------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System;
using UnityEngine;

public partial class SOWepEquipDataConfig {

    public readonly string id;
	public readonly string PI_Data;
	public readonly string SkinSign;
	public readonly string[] Bullet;
	public readonly int[] BulletNum;
	public readonly float ReloadRatio;
	public readonly float ShootRangeRatio;
	public readonly float RecoilRatio;
	public readonly float MaxLeftRightRecoilRatio;
	public readonly float CameraRatioLevel;
	public readonly float minCameraRatioLevel;
	public readonly float AimDownRatio;
	public readonly float SoundRatio;
	public readonly bool ShowFireEffect;
	public readonly bool IsSilencerAudio;
	public readonly float pressWeaponLength;
	public readonly float FireDeployTimeUpRatio;
	public readonly float BulletPowerUpRatio;
	public readonly bool IsModifyBombDamage;
	public readonly int FireBulletNum;
	public readonly int MaxDelayTimeBulletNum;
	public readonly float BulletScaleRatio;
	public readonly int CostBulletNum;
	public readonly float BombScaleRatio;
	public readonly int ZiziBulletDamage;
	public readonly float ChargeCostOneBulletTime;

    

    public SOWepEquipDataConfig(string input) {
        try {
            var tables = input.Split('\t');
            id = tables[0];
			PI_Data = tables[1];
			SkinSign = tables[2];
			Bullet = tables[3].Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);
			string[] BulletNumStringArray = tables[4].Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);
			BulletNum = new int[BulletNumStringArray.Length];
			for (int i=0;i<BulletNumStringArray.Length;i++)
			{
				 int.TryParse(BulletNumStringArray[i],out BulletNum[i]);
			}
			float.TryParse(tables[5],out ReloadRatio); 
			float.TryParse(tables[6],out ShootRangeRatio); 
			float.TryParse(tables[7],out RecoilRatio); 
			float.TryParse(tables[8],out MaxLeftRightRecoilRatio); 
			float.TryParse(tables[9],out CameraRatioLevel); 
			float.TryParse(tables[10],out minCameraRatioLevel); 
			float.TryParse(tables[11],out AimDownRatio); 
			float.TryParse(tables[12],out SoundRatio); 
			var ShowFireEffectTemp = 0;
			int.TryParse(tables[13],out ShowFireEffectTemp); 
			ShowFireEffect=ShowFireEffectTemp!=0;
			var IsSilencerAudioTemp = 0;
			int.TryParse(tables[14],out IsSilencerAudioTemp); 
			IsSilencerAudio=IsSilencerAudioTemp!=0;
			float.TryParse(tables[15],out pressWeaponLength); 
			float.TryParse(tables[16],out FireDeployTimeUpRatio); 
			float.TryParse(tables[17],out BulletPowerUpRatio); 
			var IsModifyBombDamageTemp = 0;
			int.TryParse(tables[18],out IsModifyBombDamageTemp); 
			IsModifyBombDamage=IsModifyBombDamageTemp!=0;
			int.TryParse(tables[19],out FireBulletNum); 
			int.TryParse(tables[20],out MaxDelayTimeBulletNum); 
			float.TryParse(tables[21],out BulletScaleRatio); 
			int.TryParse(tables[22],out CostBulletNum); 
			float.TryParse(tables[23],out BombScaleRatio); 
			int.TryParse(tables[24],out ZiziBulletDamage); 
			float.TryParse(tables[25],out ChargeCostOneBulletTime); 
        } catch (Exception ex) {
            DebugEx.LogError(ex);
        }
    }

    static Dictionary<string, SOWepEquipDataConfig> configs = null;
    public static SOWepEquipDataConfig Get(string id) {
        if (!Inited) {
            Init(true);
        }

        if (string.IsNullOrEmpty(id)) {
            return null;
        }

        if (configs.ContainsKey(id)) {
            return configs[id];
        }

        SOWepEquipDataConfig config = null;
        if (rawDatas.ContainsKey(id)) {
            config = configs[id] = new SOWepEquipDataConfig(rawDatas[id]);
            rawDatas.Remove(id);
        }

        if (config == null) {
            DebugEx.LogFormat("获取配置失败 SOWepEquipDataConfig id:{0}", id);
        }

        return config;
    }

    public static SOWepEquipDataConfig Get(int id) {
        return Get(id.ToString());
    }

    public static bool Has(string id) {
        if (!Inited) {
            Init(true);
        }

        return configs.ContainsKey(id) || rawDatas.ContainsKey(id);
    }

    public static List<string> GetKeys() {
        if (!Inited) {
            Init(true);
        }

        var keys = new List<string>();
        keys.AddRange(configs.Keys);
        keys.AddRange(rawDatas.Keys);
        return keys;
    }

    public static bool Inited { get; private set; }
    protected static Dictionary<string, string> rawDatas = null;
    public static void Init(bool sync = false) {
        Inited = false;
        var lines = ConfigManager.GetConfigRawDatas("SOWepEquipData");
        configs = new Dictionary<string, SOWepEquipDataConfig>();

        if (sync) {
            rawDatas = new Dictionary<string, string>(lines.Length - 3);
            for (var i = 3; i < lines.Length; i++) {
                var line = lines[i];
                var index = line.IndexOf("\t");
                var id = line.Substring(0, index);

                rawDatas.Add(id, line);
            }
            Inited = true;
        } else {
            ThreadPool.QueueUserWorkItem((object @object) => {
                rawDatas = new Dictionary<string, string>(lines.Length - 3);
                for (var i = 3; i < lines.Length; i++) {
                    var line = lines[i];
                    var index = line.IndexOf("\t");
                    var id = line.Substring(0, index);

                    rawDatas.Add(id, line);
                }

                Inited = true;
            });
        }
    }

}
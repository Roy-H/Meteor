using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInterrupt : Singleton<ActionInterrupt>
{

}

public class CommonAction
{
    public const int DartReady = 1;
    public const int GunReady = 2;
    public const int GuillotinesReady = 3;
    public const int BrahchthrustReady = 4;
    public const int KnifeReady = 5;
    public const int SwordReady = 6;
    public const int LanceReady = 7;
    public const int BladeReady = 8;
    public const int HammerReady = 9;
    public const int Reborn = 22;
    public const int DefenceHitStart = 40;//防御受击开始-不受伤，只播放动画
    public const int DefenceHitEnd = 88;//防御受击结束
    public const int HitStart = 90;//受击动作开始
    public const int HitEnd = 121;//受击动作结束
    public const int ZhihuReady = 400;
    public const int QK_BADAO_READY = 401;
    public const int QK_CHIQIANG_READY = 402;
    public const int QK_JUHE_READY = 403;
    public const int RendaoReady = 404;
    public const int WaitWeaponReturn = 219;//等待飞轮回来
    public const int AttackActStart = 200;
    public const int BeHurted109 = 109;//受击低着头循环
    public const int BeHurted110 = 110;//受击抱着肚子
    public const int BeHurted111 = 111;//打的地面上翻滚
    public const int BeHurted114 = 114;//躺在地面继续受击。
    public const int BeHurted115 = 115;//趴在地面继续受击。
    public const int Struggle0 = 112;
    public const int Struggle = 113;

    public const int DefAttack116 = 116;//防御打击
    public const int Dead = 117;
    public const int WalkForward = 140;//走路 前
    public const int WalkRight = 141;//走路 右
    public const int WalkLeft = 142;//走路 左
    public const int WalkBackward = 143;//走路 后
    public const int Run = 144;//跑步
    public const int Idle = 0;
    public const int GunReload = 212;//装载子弹
    public const int GunIdle = 213;//待发射子弹
    public const int Crouch = 10;//蹲着
    //蹲下 前右左后移动
    public const int CrouchForw = 145;
    public const int CrouchRight = 146;
    public const int CrouchLeft = 147;
    public const int CrouchBack = 148;

    //164-175 前右左后 闪 一样3个 指虎(509-512) 忍刀(501-504) 乾坤刀(505-508)
    public const int DForw1 = 164;//前闪 暗器 火枪 双刺 锤子
    public const int DForw2 = 165;//前闪 剑 刀
    public const int DForw3 = 166;//前闪 匕首 枪 飞轮
    public const int DForw4 = 501;//忍刀前闪
    public const int DForw5 = 505;//乾坤刀前闪
    public const int DForw6 = 509;//指虎前闪
    public const int DRight1 = 167;
    public const int DRight2 = 168;
    public const int DRight3 = 169;
    public const int DRight4 = 502;//忍刀右闪
    public const int DRight5 = 506;//乾坤刀右闪
    public const int DRight6 = 510;//指虎右闪
    public const int DLeft1 = 170;
    public const int DLeft2 = 171;
    public const int DLeft3 = 172;
    public const int DLeft4 = 503;//忍刀左
    public const int DLeft5 = 507;//乾坤左闪
    public const int DLeft6 = 511;//指虎左闪
    public const int DBack1 = 173;
    public const int DBack2 = 174;
    public const int DBack3 = 175;
    public const int DBack4 = 504;//忍刀后闪
    public const int DBack5 = 508;//乾坤后闪
    public const int DBack6 = 512;//指虎后闪
    //蹲下 前右左后翻滚
    public const int DCForw = 176;
    public const int DCRight = 177;
    public const int DCLeft = 178;
    public const int DCBack = 179;
    public const int JumpFallOnGround = 180;//落地
    public const int RunOnDrug = 150;//带毒走
    public const int Jump = 151;//前跳
    public const int JumpFall = 152;//前跳回落
    public const int JumpRight = 153;//右跳
    public const int JumpRightFall = 154;//右跳回落
    public const int JumpLeft = 155;//左跳
    public const int JumpLeftFall = 156;//左跳回落
    public const int JumpBack = 157;//后跳
    public const int JumpBackFall = 158;//后跳回落
    public const int WallRightJump = 159;//接触墙壁时按跳右蹬腿
    public const int WallLeftJump = 160;//接触墙壁时按跳左蹬腿
    public const int FallOnGround = 180;//落到地面时,跳回落动画还未播放完毕则播放撞击效果的落地.
    public const int Defence = 1000;//虚拟动作，因为与武器有关联
    public const int Attack = 1001;//虚拟动作，因为与武器类型有关联，攻击类的不需要自己控制，读character.act，
    //只有攀爬，
    public const int SwordAttack = 0;//剑A
    public const int BladeAttack = 0;//刀A
    public const int BreakOut = 367;
    public const int ChangeWeapon = 24;
    public const int Taunt = 32;//嘲讽.挑衅
    public const int AirChangeWeapon = 36;//空中换武器

    public const int DartDefence = 11;//飞镖防御
    public const int GuillotinesDefence = 12;//血滴子防御
    public const int MarkDefence = 13;//火铳-防御
    public const int BrahchthrustDefence = 14;//双刺防御
    public const int KnifeDefence = 15; //匕首-防御 
    public const int SwordDefence = 16;//剑-防御
    public const int LanceDefence = 17;//长矛-防御.
    public const int BladeDefence = 18;//大刀-防御
    public const int HammerDefence = 19;//锤子-防御
    public const int ZhihuDefence = 480;//指虎防御
    public const int QiankunDefenct = 481;//乾坤刀防御
    public const int RendaoDefence = 482;//忍刀防御
                                         //475-477指虎防御受击
    public const int OnDrugHurt = 90;//挨打后仰
                                     //其他的呢?
    public const int ClimbUp = 161;
    public const int ClimbLeft = 162;
    public const int ClimbRight = 163;

    public const int KnifeWW = 253;//匕首地面上上A
    public const int KnifeSkill = 259;

    public const int KnifeA2Fall = 332;//空中A后落下 算跳跃落地POSE，可以继续爬墙
    public const int HammerMaxFall = 328;//空中大招后落下
    public const int Fall = 118;
}

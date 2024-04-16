using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace gamedata_c_
{
    internal class Program
    {
        const float MAX_PROBABILITY = 100.0f;
        const int WAIT_TIME = 2;

        // 강화 변수
        static int isTry = 0;
        static int level = 0;
        static int totallevel = 0;
        static float probability = MAX_PROBABILITY;
        static int support = 0;
        static bool nobreak = false;
        static Random random = new Random();

        // 상점 변수
        static int money = 500;
        static int weaponvalue = 0;

        // 각 아이템에 대한 구입 횟수를 저장하는 배열
        static int[] itemPurchaseCounts = new int[3]; // 3개의 아이템의 구입 횟수 저장 변수



        static void Main()
        {
            //게임 실행 시작 시간 저장 - csv
            WriteToCsv("game_start_time.csv", "Game Start Time", DateTime.Now, 1);

            
            // 아이템 구입 횟수 초기화
            for (int i = 0; i < itemPurchaseCounts.Length; i++)
            {
                itemPurchaseCounts[i] = 0;
            }

            while (true)
            {
                Console.Clear();

                // 타이틀 화면 출력
                Console.WriteLine("*= Blacksmith =*");
                Console.WriteLine("  -----------\n");

                // 현재 상태와 강화 도전 질의 출력
                Console.WriteLine($"재화 : {money}골드\n");
                Console.WriteLine($"무기레벨 : + {level}");
                Console.WriteLine($"성공확률 : {probability + support:F2}%");
                Console.WriteLine("도전하시겠습니까?");
                Console.WriteLine("1.강화  2. 판매  3. 주문서 4. 포기 5. 최고기록");
                isTry = int.Parse(Console.ReadLine());

                switch (isTry)
                {
                    case 1:
                        Enforce();
                        break;
                    case 2:
                        Shop();
                        break;
                    case 3:
                        EnforceSupport();
                        break;
                    case 4:
                        Console.WriteLine("게임을 종료합니다");
                        WriteToCsv("item_usage.csv", "Item Type", "무기 파괴 방지서 사용 횟수 : ", itemPurchaseCounts[0]);
                        WriteToCsv("item_usage.csv", "Item Type", "강화 확률 상승서 사용 횟수 : ", itemPurchaseCounts[1]);
                        WriteToCsv("item_usage.csv", "Item Type", "무기 가치 상승서 사용 횟수 : ", itemPurchaseCounts[2]);
                        return;
                    case 5:
                        Console.WriteLine($"무기의 최고레벨은 {totallevel} 입니다.");
                        break;
                }

                Console.WriteLine("\n계속하려면 아무 키나 누르십시오.");
                Console.ReadKey();
            }
        }

        static void Enforce()
        {
            Console.WriteLine("\n강화중..\n");

            System.Threading.Thread.Sleep(WAIT_TIME * 1000);

            int randNum = random.Next(100);

            if (randNum < probability + support)
            {
                Console.WriteLine("***** SUCCESS *****");
                Console.WriteLine($"*                 *");
                Console.WriteLine($"*   + {level}  ->  + {level + 1}  *");
                Console.WriteLine($"*                 *");
                Console.WriteLine("***** SUCCESS *****");

                level++;
                if (totallevel < level)
                {
                    totallevel = level;
                }
                support = 0;
                weaponvalue += 500;
                probability -= (probability / 10.0f) * level;
            }
            else
            {
                Console.WriteLine("강화 실패..");
                if (nobreak == true)
                {
                    Console.WriteLine("절대 파괴 안 됨의 가호가 치명적인 파괴를 막았습니다.");
                    Console.WriteLine("가호가 사라집니다.");
                    nobreak = false;
                }
                else
                {
                    Console.WriteLine($"+ {level} 무기를 잃었습니다.");

                    level = 0;
                    support = 0;
                    weaponvalue = 0;
                    probability = MAX_PROBABILITY;
                }
            }
        }

        static void EnforceSupport()
        {
            Console.WriteLine("신비의 주문서\n");
            Console.WriteLine($"1. 무기 파괴 방지서 : {100 + (level * 100)}골드");
            Console.WriteLine($"2. 강화 확률 상승서 : {200 + (level * 200)}골드");
            Console.WriteLine($"3. 무기 가치 상승서 : {50 + (level * 50)}골드");
            int supportchoice = int.Parse(Console.ReadLine());

            switch (supportchoice)
            {
                case 1:
                    if (money >= level * 100)
                    {
                        if (nobreak == false)
                        {
                            Console.WriteLine("무기 파괴 방지서를 구매하였습니다.");
                            Console.WriteLine("절대 파괴 안 됨의 가호가 깃듭니다.");
                            nobreak = true;
                            money -= 100 + (level * 100);
                            itemPurchaseCounts[0] += 1; // 첫 번째 아이템 구입 횟수 증가

                        }
                        else
                        {
                            Console.WriteLine("이미 받고 있는 가호입니다.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("재화가 부족합니다.");
                    }
                    break;
                case 2:
                    if (money >= level * 200)
                    {
                        Console.WriteLine("강화 확률 상승서를 구매하였습니다.");
                        Console.WriteLine("손이 미끄러지는 기운이 멀어집니다.");
                        Console.WriteLine($"성공확률 : {probability + support:F2}%");
                        support += 5;
                        money -= 200 + (level * 100);
                        itemPurchaseCounts[1] += 1; // 두 번째 아이템 구입 횟수 증가

                    }
                    else
                    {
                        Console.WriteLine("재화가 부족합니다.");
                    }
                    break;
                case 3:
                    if (money >= level * 50)
                    {
                        Console.WriteLine("무기 가치 상승서를 구매하였습니다.");
                        weaponvalue += (50 * level);
                        money -= 50 + (level * 100);
                        Console.WriteLine("당신에게 재화의 가호가 깃들었습니다.!!");
                        Console.WriteLine($"현재 무기 가치 보정 : {weaponvalue}\n");
                        itemPurchaseCounts[2] += 1; // 세 번째 아이템 구입 횟수 증가

                    }
                    else
                    {
                        Console.WriteLine("재화가 부족합니다.");
                    }
                    break;
                default:
                    Console.WriteLine("잘못된 선택입니다.");
                    break;
            }
        }

        static void Shop()
        {
            if (level > 0)
            {
                int sellvalue = weaponvalue / 2;

                Console.WriteLine("무기를 판매합니다.");
                Console.WriteLine($"판매된 무기 레벨: + {level}");
                Console.WriteLine($"판매하는 무기의 값: {sellvalue}골드");

                money += sellvalue;
                level = 0;
                weaponvalue = 0;
                probability = MAX_PROBABILITY;
            }
            else
            {
                Console.WriteLine("판매할 무기가 없습니다.");
            }
        }

        //csv 파일에 저장
        static void WriteToCsv(string fileName, string item, object value, int number)
        {
            // CSV 파일에 기록
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                if (value is int)
                {
                    writer.WriteLine($"{item},{value}");
                }
                else if (value is string)
                {
                    writer.WriteLine($"{item},{value},{number}"); //아이템 종류 및 구입 횟수 저장
                }
                else if (value is DateTime)
                {
                    writer.WriteLine($"{item},{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}"); //시간 계통 저장
                }
            }
        }
    }
}

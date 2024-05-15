using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;
using static gamedata_c_.Program;

namespace gamedata_c_
{
    internal class Program
    {
        const float MAX_PROBABILITY = 100.0f;
        const int WAIT_TIME = 2;

        // 강화 변수
        static int isTry = 0;
        static int level = 0; //현재 무기 강화 수치
        
        static float probability = MAX_PROBABILITY;
        static int support = 0;
        static bool nobreak = false;
        static Random random = new Random();

        // 상점 변수
        static int money = 500;
        static int weaponvalue = 0;

        



        static async Task Main()
        {
            // 플레이어 이름 입력 받기
            Console.Write("게임을 시작하기 전에 플레이어 명을 입력하세요: ");
            string playerName = Console.ReadLine();

            // 게임 실행 시작 시간 저장
            DateTime startTime = DateTime.Now;
            // 플레이어 정보 초기화
            PlayerData playerData = new PlayerData(playerName);

            // 플레이어에게 환영 메시지 출력
            Console.WriteLine($"\n\n안녕하세요, {playerName}님! 게임을 시작합니다.");
            // 1초 대기
            Thread.Sleep(1000);

            // 게임 실행
            Task gameTask = Task.Run(() => gamemain(playerName, startTime, playerData));

           

            // 대기
            await gameTask;
            Console.ReadLine();


        }

        // 플레이어 정보 클래스
        public class PlayerData
        {
            public string Name { get; set; } //플레이어 명
            public DateTime StartTime { get; set; } //로그인 시간
            public DateTime EndTime { get; set; } //로그아웃 시간
            public double ElapsedSeconds { get; set; } //플레이 타임
            public int MaxUpgradeLevel { get; set; } //최고강화수치
            public int TotalMoneySpent { get; set; } //소모한 재화량
            public int TotalUpgrades { get; set; } //강화횟수
            public double AverageUpgradeLevel { get { return TotalUpgrades == 0 ? 0 : (double)TotalUpgradeValue / TotalUpgrades; } } //평균강화수치
            public int TotalUpgradeValue { get; set; } //총강화수치

            public int[] ItemPurchaseCounts { get; set; } // 각 아이템에 대한 구입 횟수를 저장하는 배열
            public PlayerData(string name)
            {
                Name = name;
                StartTime = DateTime.Now;
                ItemPurchaseCounts = new int[3]; // 예시로 3개의 아이템을 추적하도록 설정
            }
        }

        static void EndGame(string playerName, DateTime startTime, PlayerData playerData)
        {
            // 게임 종료 시간 및 경과 시간 업데이트
            playerData.EndTime = DateTime.Now;
            playerData.ElapsedSeconds = (playerData.EndTime - startTime).TotalSeconds;

            // 데이터 기록
            WriteGameData(playerData);
        }

        static async Task WriteGameData(PlayerData playerData)
        {
            

            // 데이터를 JSON 형식으로 직렬화합니다.
            var data = new
            {
                username = playerData.Name, //유저명
                startTime = playerData.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"), //로그인시간
                endTime = playerData.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"), //로그아웃 시간
                elapsedTimeSeconds = playerData.ElapsedSeconds, //플레이타임
                maxUpgradeLevel = playerData.MaxUpgradeLevel, //최고강화수치
                averageUpgradeLevel = playerData.AverageUpgradeLevel, //평균강화수치
                totalMoneySpent = playerData.TotalMoneySpent, //소모한 재화량
                itemPurchaseCounts = playerData.ItemPurchaseCounts // 아이템 구입 횟수

            };
            string jsonData = JsonConvert.SerializeObject(data);

            // 앱 스크립트 웹 앱의 URL
            string scriptUrl = "https://script.google.com/macros/s/AKfycbybYC17LSOMPgOnuvufOrHUktdAwaKAN1zPu8WlbOQxq9J3UeauQL8zsFpgt21VS67qsQ/exec";

            // HttpClient 인스턴스 생성
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // HTTP POST 요청 보내기
                    HttpResponseMessage response = await client.PostAsync(scriptUrl, new StringContent(jsonData, Encoding.UTF8, "application/json"));

                    // 응답 확인
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("데이터가 성공적으로 전송되었습니다.");

                        // 응답 본문을 문자열로 읽어옵니다.
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("앱 스크립트 응답: " + responseContent);
                    }
                    else
                    {
                        Console.WriteLine($"에러 코드: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"에러 발생: {ex.Message}");
                }
            }
            Console.WriteLine(jsonData);
            Console.ReadLine();
        }
        static void gamemain(string playerName, DateTime startTime, PlayerData playerData)
        {
            
            probability = probability + support;
            if (probability > 100)
            {
                probability = 100; //강화 확률 상한치 
            }

            while (true)
            {




                // 선택지 표시 및 선택
                ConsoleKeyInfo keyPressed;
                int selectedOption = 1;
                do
                {
                    // 타이틀 화면 출력
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=\n");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("                                         ");
                    Console.WriteLine("               Blacksmith                ");
                    Console.WriteLine("                                         ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    // 현재 상태와 강화 도전 질의 출력
                    Console.WriteLine("\n=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=\n");
                    Console.ResetColor();
                    Console.WriteLine("                           현재 상태\n");
                    Console.WriteLine($"                      재화 : {money} 골드");
                    Console.WriteLine($"                      무기 레벨 : +{level}");
                    Console.WriteLine($"                      성공 확률 : {probability + support:F2}%\n");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=\n");
                    Console.ResetColor();







                    Console.SetCursorPosition(0, selectedOption + 12);
                    Console.ForegroundColor = ConsoleColor.White;
                    for (int i = 1; i <= 5; i++)
                    {
                        Console.SetCursorPosition(0, i + 7);
                        if (i == selectedOption)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;


                        }
                        else
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        switch (i)
                        {
                            case 1:
                                Console.WriteLine(" 1. 강화");

                                break;
                            case 2:
                                Console.WriteLine(" 2. 판매");
                                break;
                            case 3:
                                Console.WriteLine(" 3. 주문서");
                                break;
                            case 4:
                                Console.WriteLine(" 4. 게임종료");
                                break;
                            case 5:
                                Console.WriteLine(" 5. 최고 기록");
                                break;
                        }
                    }
                    Console.ResetColor();

                    keyPressed = Console.ReadKey();

                    if (keyPressed.Key == ConsoleKey.UpArrow)
                    {
                        if (selectedOption > 1)
                        {
                            selectedOption--;
                        }
                    }
                    else if (keyPressed.Key == ConsoleKey.DownArrow)
                    {
                        if (selectedOption < 5)
                        {
                            selectedOption++;
                        }
                    }

                } while (keyPressed.Key != ConsoleKey.Enter);
                Console.SetCursorPosition(0, 16);


                // 선택 실행
                switch (selectedOption)
                {
                    case 1:
                        Enforce(playerData);
                        break;
                    case 2:
                        Shop(playerData);
                        break;
                    case 3:
                        EnforceSupport(1, playerData);
                        break;
                    case 4:
                        Console.WriteLine("게임을 종료합니다");
                        EndGame(playerName, startTime, playerData); // 게임 종료 시 호출
                        return;
                    case 5:
                        Console.WriteLine($"무기의 최고레벨은 {playerData.MaxUpgradeLevel} 입니다.");
                        Console.WriteLine($"평균 강화 수치: {playerData.AverageUpgradeLevel:F2}");
                        break;
                }



                Console.WriteLine("\n계속하려면 아무 키나 누르십시오.");
                Console.ReadKey();
            }
        }

        static void Enforce(PlayerData playerData)
        {
            Console.WriteLine("\n강화중....\n");

            System.Threading.Thread.Sleep(WAIT_TIME * 500);

            int randNum = random.Next(100);

            if (randNum < probability + support)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("***** SUCCESS *****");
                Console.WriteLine($"*                 *");
                Console.WriteLine($"*   + {level}  ->  + {level + 1}  *");
                Console.WriteLine($"*                 *");
                Console.WriteLine("***** SUCCESS *****");
                Console.ResetColor();

                level++;
                // 최고 강화 수치 업데이트
                if (level > playerData.MaxUpgradeLevel)
                {
                    playerData.MaxUpgradeLevel = level;
                }
                // 총 강화 수치와 횟수 업데이트
                playerData.TotalUpgradeValue += level;
                playerData.TotalUpgrades++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("강화 실패..");
                Console.ResetColor();

                if (nobreak == true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("절대 파괴 안 됨의 가호가 치명적인 파괴를 막았습니다.");
                    Console.WriteLine("가호가 사라집니다.");
                    Console.ResetColor();
                    nobreak = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"+ {level} 무기를 잃었습니다.");
                    Console.ResetColor();

                    level = 0;
                    support = 0;
                    weaponvalue = 0;
                    probability = MAX_PROBABILITY;
                }
            }
        }

        static void EnforceSupport(int selectedOption, PlayerData playerData)
        {
            ConsoleKeyInfo keyPressed;

            do
            {
                Console.Clear();
                Console.WriteLine("\n             ** 신비의 주문서 상점 **                    \n");
                Console.WriteLine("********************** SHOP ****************************");
                Console.WriteLine("*                                                      *");
                Console.ForegroundColor = ConsoleColor.Blue;

                int[] itemPrices = new int[]
                {
        100 + (level * 100),
        200 + (level * 200),
        50 + (level * 50)
                };

                string[] itemDescriptions = new string[]
                {
        "무기 파괴 방지서",
        "강화 확률 상승서",
        "무기 가치 상승서"
                };

                for (int i = 0; i < itemPrices.Length; i++)
                {
                    Console.Write($"        ┌   {i + 1}. {itemDescriptions[i]} : {itemPrices[i]}골드");
                    if (i + 1 == selectedOption)
                    {
                        
                        Console.WriteLine(" <- 선택");
                    }
                    else
                    {
                        
                        Console.WriteLine();
                    }
                }

                Console.ResetColor();
                Console.WriteLine("*                                                      *");
                Console.WriteLine("********************** SHOP ****************************");

                keyPressed = Console.ReadKey();

                if (keyPressed.Key == ConsoleKey.UpArrow)
                {
                    if (selectedOption > 1)
                    {
                        selectedOption--;
                    }
                }
                else if (keyPressed.Key == ConsoleKey.DownArrow)
                {
                    if (selectedOption < itemPrices.Length)
                    {
                        selectedOption++;
                    }
                }
            } while (keyPressed.Key != ConsoleKey.Enter);

            switch (selectedOption)
            {
                case 1:
                    if (money >= level * 100)
                    {
                        if (nobreak == false)
                        {
                            Console.WriteLine("┌ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ┐");
                            Console.WriteLine("  무기 파괴 방지서를 구매하였습니다.\n");
                            Console.WriteLine("  절대 파괴 안 됨의 가호가 깃듭니다.");
                            Console.WriteLine("└ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ┘");
                            nobreak = true;
                            money -= 100 + (level * 100);
                            playerData.TotalMoneySpent += 100 + (level * 100);
                            playerData.ItemPurchaseCounts[0]++; // 첫 번째 아이템 구입 횟수 증가

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
                        Console.WriteLine("┌ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ┐");
                        Console.WriteLine("  강화 확률 상승서를 구매하였습니다.\n");
                        Console.WriteLine("  손이 미끄러지는 기운이 멀어집니다.");
                        Console.WriteLine("└ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ┘");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"       ** 성공확률 : {probability + support:F2}% **");
                        Console.ResetColor();
                        support += 5;
                        playerData.TotalMoneySpent += 200 + (level * 100);
                        playerData.ItemPurchaseCounts[1]++; // 첫 번째 아이템 구입 횟수 증가

                    }
                    else
                    {
                        Console.WriteLine("재화가 부족합니다.");
                    }
                    break;
                case 3:
                    if (money >= level * 50)
                    {
                        Console.WriteLine("┌ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ┐");
                        Console.WriteLine("  무기 가치 상승서를 구매하였습니다.\n");
                        weaponvalue += (50 * level);
                        playerData.TotalMoneySpent += 50 + (level * 100);
                        Console.WriteLine("  당신에게 재화의 가호가 깃들었습니다.!!");
                        Console.WriteLine("└ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ┘");
                        Console.WriteLine($"      ** 현재 무기 가치 보정 : {weaponvalue} **\n");
                        playerData.ItemPurchaseCounts[2]++; // 첫 번째 아이템 구입 횟수 증가

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

        static void Shop(PlayerData playerData)
        {
            if (level > 0)
            {
                int sellvalue = weaponvalue / 2;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("===================");
                Console.WriteLine("     무기 판매");
                Console.WriteLine("===================");
                Console.ResetColor();
                Console.WriteLine($"무기 레벨: +{level}");
                Console.WriteLine($"판매 가격: {sellvalue}골드\n");
                Console.WriteLine("판매하시겠습니까? (y/n)");

                // 사용자 입력 확인
                string input = Console.ReadLine();
                if (input.ToLower() == "y")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"무기 레벨 +{level}을(를) 판매했습니다. {sellvalue}골드를 획득하였습니다.");
                    Console.ResetColor();
                    money += sellvalue;
                    level = 0;
                    weaponvalue = 0;
                    probability = MAX_PROBABILITY;
                    
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("판매가 취소되었습니다.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("\n판매할 무기가 없습니다.");
            }
        }

        
    }
}

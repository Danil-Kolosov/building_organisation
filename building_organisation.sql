-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Хост: localhost:3306
-- Время создания: Май 14 2025 г., 23:59
-- Версия сервера: 8.0.39
-- Версия PHP: 8.1.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `building_organisation`
--

DELIMITER $$
--
-- Процедуры
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_brigade_on_work_type_with_date_and_objects` (`work_type_name` VARCHAR(45), `start_date` DATE, `end_date` DATE)   BEGIN
    SELECT 
        b.BrigadeID,
        b.BrigadeName,
        wt.WorkTypeName,
        o.ObjectName,
        o.ObjectNameID,
        w.WorkNumber,
        w.PlannedStartDate,
        w.PlannedEndDate,
        w.RealStartDate,
        w.RealEndDate,
        CASE 
            WHEN w.RealStartDate IS NOT NULL AND w.RealEndDate IS NOT NULL 
            THEN DATEDIFF(w.RealEndDate, w.RealStartDate)
            ELSE DATEDIFF(w.PlannedEndDate, w.PlannedStartDate)
        END AS Duration
    FROM 
        work w
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN brigade b ON w.BrigadeID = b.BrigadeID
    WHERE 
        wt.WorkTypeName = work_type_name
        AND (
            -- Работы, которые должны были выполняться в указанный период (по плану)
            (w.PlannedStartDate <= end_date AND w.PlannedEndDate >= start_date)
            OR
            -- Или фактически выполнялись в указанный период
            (w.RealStartDate IS NOT NULL AND w.RealStartDate <= end_date AND 
             (w.RealEndDate IS NULL OR w.RealEndDate >= start_date))
        )
    ORDER BY 
        b.BrigadeName,
        o.ObjectName,
        COALESCE(w.RealStartDate, w.PlannedStartDate);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_engineering_employee_management` (`management_number` INT)   BEGIN

SELECT 
        s.SectionNameID, 
        e.FirstName, 
        e.LastName, 
        e.HireDate, 
        g.GroupName AS Position -- Называем столбец как "Position" для ясности
    FROM employee e
    -- Соединяем с таблицей "section_employee" (какие участки у сотрудника)
    JOIN section_employee se ON e.EmployeeCode = se.EmployeeCode  
    -- Соединяем с таблицей "section" (участки)
    JOIN section s ON se.SectionNameID = s.SectionNameID  
    -- Соединяем с таблицей "management" (управления)
    JOIN management m ON s.ManagementNumber = m.ManagementNumber  
    -- Соединяем с таблицей "group" (должности)
    JOIN building_organisation.group g ON e.GroupNameID = g.GroupNameID
    -- Фильтруем по нужному управлению
    WHERE m.ManagementNumber = management_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_engineering_employee_section` (`section_number` INT)   BEGIN
SELECT employee.FirstName, employee.LastName, employee.HireDate, g.GroupName AS Position
    FROM employee, section_employee, building_organisation.group g
    WHERE employee.EmployeeCode = section_employee.EmployeeCode 
    AND section_employee.SectionNameID = section_number
    AND employee.GroupNameID = g.GroupNameID;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_machine_by_object` (`object_number` INT)   BEGIN

#SELECT SerialNumber, machine_type.MachineType
#FROM machine, machine_type, object, section
#WHERE object.ObjectNameID = object_number
#AND object.SectionNameID = section.SectionNameId 
#AND machine.ManagementNumber = section.ManagementNumber  
#AND machine_type.MachineTypeID = machine.MachineTypeID;

SELECT machine.SerialNumber, machine_type.MachineType, PlannedStartDate, PlannedEndDate, RealStartDate, RealEndDate
FROM machine, machine_type, work, work_machine
WHERE work.ObjectNameID = object_number 
AND work.WorkNumber = work_machine.WorkNumber
AND work_machine.SerialNumber = machine.SerialNumber
AND machine_type.MachineTypeID = machine.MachineTypeID;


END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_machine_by_object_by_date` (`object_number` INT, `first_date` DATE, `second_date` DATE)   BEGIN

SELECT machine.SerialNumber, machine_type.MachineType, PlannedStartDate, PlannedEndDate, RealStartDate, RealEndDate
FROM machine, machine_type, work, work_machine
WHERE work.ObjectNameID = object_number 
AND work.WorkNumber = work_machine.WorkNumber
AND work_machine.SerialNumber = machine.SerialNumber
AND machine_type.MachineTypeID = machine.MachineTypeID

AND ((work.PlannedStartDate >= first_date OR work.RealStartDate >= first_date) 
OR (work.PlannedEndDate <= second_date OR work.RealEndDate <= second_date)) 
;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_managements_with_boss` ()   BEGIN
SELECT ManagementNumber, FirstName, LastName, HireDate, GroupName
FROM management, employee, building_organisation.group g
WHERE management.Director = employee.EmployeeCode AND employee.GroupNameID = g.GroupNameID;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_management_machines` (`management_number` INT)   BEGIN

SELECT SerialNumber, machine_type.MachineType
FROM machine, machine_type
WHERE machine.ManagementNumber = management_number  
AND machine_type.MachineTypeID = machine.MachineTypeID;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_objects_by_section` (`sectionID` INT)   BEGIN
SELECT 
        m.ManagementNumber,
        s.SectionName,
        o.ObjectNameID,
        o.ObjectName,
        w.WorkNumber,
        wt.WorkTypeName,
        w.PlannedStartDate,
        w.PlannedEndDate
    FROM 
        object o
    JOIN work w ON o.ObjectNameID = w.ObjectNameID
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management m ON s.ManagementNumber = m.ManagementNumber
    WHERE 
        s.SectionNameID = sectionID
    ORDER BY 
        o.ObjectName,
        w.PlannedStartDate;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_objects_schedules_by_section` (`sectionID` INT)   BEGIN
SELECT 
        m.ManagementNumber,# AS 'Управление',
        s.SectionName,# AS 'Участок',
        o.ObjectNameID,# AS 'ID объекта',
        o.ObjectName,# AS 'Название объекта',
        w.WorkNumber,# AS 'Номер работы',
        wt.WorkTypeName,# AS 'Тип работы',
        w.PlannedStartDate,# AS 'План. начало',
        w.PlannedEndDate# AS 'План. окончание'
    FROM 
        object o
    JOIN work w ON o.ObjectNameID = w.ObjectNameID
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management m ON s.ManagementNumber = m.ManagementNumber
    WHERE 
        s.SectionNameID = sectionID
    ORDER BY 
        o.ObjectName,
        w.PlannedStartDate;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_objects_schedule_estimate` (`object_number` INT)   BEGIN
# каком порядке и в какие сроки выполняются работы
SELECT WorkNumber, WorkTypeName, PlannedStartDate, PlannedEndDate
FROM work, work_type
#WHERE work.ObjectNameID = object_number
JOIN material.MaterialName, material.MeasurementUnits, estimate.Cost, estimate.PlannedQuantity #ON estimate.WorkNumber = work.WorkNumber
WHERE esetimate.MaterialID = material.MaterialID
AND estimate.WorkNumber = work.WorkNumber
;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_objects_schedule_estimate_aggregated` (`object_number` INT)   BEGIN
    SELECT 
    w.WorkNumber,
    wt.WorkTypeName,
    w.PlannedStartDate,
    w.PlannedEndDate,
    GROUP_CONCAT(
        CONCAT(m.MaterialName, ' (', e.PlannedQuantity, ' ', m.MeasurementUnits, ')')
        SEPARATOR ', '
    ) AS Materials,
    SUM(e.Cost * e.PlannedQuantity) AS TotalCost
FROM work w
JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
LEFT JOIN estimate e ON w.WorkNumber = e.WorkNumber
LEFT JOIN material m ON e.MaterialID = m.MaterialID
WHERE w.ObjectNameID = object_number
GROUP BY w.WorkNumber, wt.WorkTypeName, w.PlannedStartDate, w.PlannedEndDate;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_objects_with_works_on_date` (`workTypeName` VARCHAR(45), `first_date` DATE, `second_date` DATE)   BEGIN

SELECT object.ObjectNameID, employee.FirstName, employee.LastName, section.SectionName, object_type.ObjectType, ObjectName, object.ContractNumber, Price, CustomerName, work_type.WorkTypeName, work.PlannedStartDate, work.PlannedEndDate
FROM object, contract, object_type, work, work_type, employee, section
WHERE object_type.ObjectTypeID = object.ObjectTypeID AND contract.ContractNumber = object.ContractNumber
AND work.ObjectNameID = object.ObjectNameID AND employee.EmployeeCode = object.Supervisor AND object.SectionNameID = section.SectionNameID

AND work.WorkTypeID = work_type.WorkTypeID AND work_type.WorkTypeName = workTypeName 
AND ((work.PlannedStartDate <= second_date OR work.RealStartDate <= second_date) 
OR (work.PlannedEndDate >= first_date OR work.RealEndDate >= first_date));

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_objects_with_works_on_date_in_management` (`workTypeName` VARCHAR(45), `first_date` DATE, `second_date` DATE, `management_number` VARCHAR(45))   BEGIN
SELECT object.ObjectNameID, employee.FirstName, employee.LastName, section.SectionName, object_type.ObjectType, ObjectName, object.ContractNumber, Price, CustomerName, work_type.WorkTypeName, work.PlannedStartDate, work.PlannedEndDate
FROM object, contract, object_type, work, work_type, employee, section, management
WHERE object_type.ObjectTypeID = object.ObjectTypeID AND contract.ContractNumber = object.ContractNumber
AND work.ObjectNameID = object.ObjectNameID AND employee.EmployeeCode = object.Supervisor AND object.SectionNameID = section.SectionNameID

AND work.WorkTypeID = work_type.WorkTypeID AND work_type.WorkTypeName = workTypeName 
AND ((work.PlannedStartDate <= second_date OR work.RealStartDate <= second_date) 
OR (work.PlannedEndDate >= first_date OR work.RealEndDate >= first_date))

AND section.ManagementNumber = management_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_object_brigade_members` (`object_number` INT)   BEGIN

SELECT brigade.BrigadeID, brigade.BrigadeName, FirstName, LastName, GroupName
FROM employee, building_organisation.group g, brigade, work
WHERE work.ObjectNameID = object_number AND work.BrigadeId = brigade.BrigadeID
AND employee.EmployeeCode = brigade.Foreman
AND employee.GroupNameId = g.GroupNameID

UNION SELECT brigade_employee.BrigadeID, brigade.BrigadeName, FirstName, LastName, GroupName
FROM employee, building_organisation.group g, brigade, work, brigade_employee
WHERE work.ObjectNameID = object_number AND work.BrigadeId = brigade.BrigadeID
AND employee.EmployeeCode = brigade_employee.EmployeeCode
AND work.BrigadeID = brigade_employee.BrigadeID
AND employee.GroupNameId = g.GroupNameID
order by BrigadeID;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_object_report` (`object_number` INT)   BEGIN

SELECT 
    w.WorkNumber,
    wt.WorkTypeName,
    w.PlannedStartDate,
    w.PlannedEndDate,
    w.RealStartDate,
    w.RealEndDate,
    GROUP_CONCAT(
        CONCAT(m.MaterialName, ' (', e.RealQuantity, ' ', m.MeasurementUnits, ')')
        SEPARATOR ', '
    ) AS Materials,
    SUM(e.Cost * e.RealQuantity) AS RealCost,
    SUM(e.Cost * e.RealQuantity) - SUM(e.Cost * e.PlannedQuantity) AS CostDifference,
    w.RealStartDate - w.PlannedStartDate AS StartDateDifference,
    w.RealEndDate - w.PlannedEndDate AS EndDateDifference
FROM work w
JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
LEFT JOIN estimate e ON w.WorkNumber = e.WorkNumber
LEFT JOIN material m ON e.MaterialID = m.MaterialID
WHERE w.ObjectNameID = object_number
GROUP BY w.WorkNumber, wt.WorkTypeName, w.PlannedStartDate, w.PlannedEndDate, w.RealStartDate, w.RealEndDate;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_object_schedules` (`object_number` INT)   BEGIN

SELECT object.ObjectName, WorkNumber, WorkTypeName
FROM object, work, work_type
WHERE work.ObjectNameID = object_number AND object.ObjectNameID = object_number; 


END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_object_schedules_management` (`managementNumber` INT)   BEGIN
    -- Объекты и графики работ для указанного управления
    SELECT 
        m.ManagementNumber,
        s.SectionName,
        o.ObjectNameID,
        o.ObjectName,
        w.WorkNumber,
        wt.WorkTypeName,
        w.PlannedStartDate,
        w.PlannedEndDate
    FROM 
        object o
    JOIN work w ON o.ObjectNameID = w.ObjectNameID
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management m ON s.ManagementNumber = m.ManagementNumber
    WHERE 
        m.ManagementNumber = managementNumber
    ORDER BY 
        s.SectionName,
        o.ObjectName,
        w.PlannedStartDate; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_sections_with_boss` ()   BEGIN

SELECT SectionName, ManagementNumber, FirstName, LastName, HireDate, GroupName
FROM section, employee, building_organisation.group g
WHERE section.Manager = employee.Employeecode AND employee.GroupNameID = g.GroupNameID;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_material_late_all` ()   BEGIN
    SELECT 
        m.MaterialID,
        m.MaterialName,
        mg.ManagementNumber,
        s.SectionName,
        SUM(e.PlannedQuantity) AS PlannedQuantity,
        SUM(e.RealQuantity) AS RealQuantity ,
        (SUM(e.RealQuantity) - SUM(e.PlannedQuantity)) AS Difference,
        COUNT(*) AS Count 
    FROM 
        estimate e
    JOIN material m ON e.MaterialID = m.MaterialID
    JOIN work w ON e.WorkNumber = w.WorkNumber
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management mg ON s.ManagementNumber = mg.ManagementNumber
    WHERE 
        e.RealQuantity > e.PlannedQuantity
    GROUP BY 
        m.MaterialID, m.MaterialName, mg.ManagementNumber, s.SectionName
    ORDER BY 
        mg.ManagementNumber, s.SectionName, (SUM(e.RealQuantity) - SUM(e.PlannedQuantity)) DESC;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_material_late_management` (`management_num` INT)   BEGIN
    SELECT 
        m.MaterialID,
        m.MaterialName,
        s.SectionName,
        SUM(e.PlannedQuantity) AS PlannedQuantity ,
        SUM(e.RealQuantity) AS RealQuantity ,
        (SUM(e.RealQuantity) - SUM(e.PlannedQuantity)) AS Difference ,
        COUNT(*) AS Count 
    FROM 
        estimate e
    JOIN material m ON e.MaterialID = m.MaterialID
    JOIN work w ON e.WorkNumber = w.WorkNumber
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management mg ON s.ManagementNumber = mg.ManagementNumber
    WHERE 
        mg.ManagementNumber = management_num
        AND e.RealQuantity > e.PlannedQuantity
    GROUP BY 
        m.MaterialID, m.MaterialName, s.SectionName
    ORDER BY 
        s.SectionName, (SUM(e.RealQuantity) - SUM(e.PlannedQuantity)) DESC;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_material_late_section` (`section_name` VARCHAR(45))   BEGIN
    SELECT 
        m.MaterialID,
        m.MaterialName,
        m.MeasurementUnits,
        SUM(e.PlannedQuantity) AS PlannedQuantity,
        SUM(e.RealQuantity) AS RealQuantity ,
        (SUM(e.RealQuantity) - SUM(e.PlannedQuantity)) AS Difference,
        COUNT(*) AS Count
    FROM 
        estimate e
    JOIN material m ON e.MaterialID = m.MaterialID
    JOIN work w ON e.WorkNumber = w.WorkNumber
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    WHERE 
        s.SectionName = section_name
        AND e.RealQuantity > e.PlannedQuantity
    GROUP BY 
        m.MaterialID, m.MaterialName, m.MeasurementUnits
    ORDER BY 
        (SUM(e.RealQuantity) - SUM(e.PlannedQuantity)) DESC;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_work_type_by_brigade_on_date_and_objects` (`brigade_name` VARCHAR(45), `start_date` DATE, `end_date` DATE)   BEGIN
    SELECT 
        b.BrigadeName,
        wt.WorkTypeName,
        o.ObjectName,
        o.ObjectNameID,
        w.WorkNumber,
        w.PlannedStartDate,
        w.PlannedEndDate,
        w.RealStartDate,
        w.RealEndDate,
        DATEDIFF(w.RealEndDate, w.RealStartDate) AS RealDurationDays,
        CASE 
            WHEN w.RealEndDate > w.PlannedEndDate THEN 'Yes'
            ELSE 'No'
        END AS 'Delay'
    FROM 
        work w
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN brigade b ON w.BrigadeID = b.BrigadeID
    WHERE 
        b.BrigadeName = brigade_name
        AND (
            (w.RealStartDate BETWEEN start_date AND end_date)
            OR 
            (w.RealEndDate BETWEEN start_date AND end_date)
            OR 
            (w.RealStartDate <= start_date AND w.RealEndDate >= end_date)
        )
    ORDER BY 
        w.RealStartDate,
        o.ObjectName,
        wt.WorkTypeName;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_work_type_late_all` ()   BEGIN
    SELECT 
        wt.WorkTypeID,
        wt.WorkTypeName,
        m.ManagementNumber,
        s.SectionName,
        COUNT(*) AS LateCount,
        AVG(DATEDIFF(w.RealEndDate, w.PlannedEndDate)) AS AvgDelayDays
    FROM 
        work w
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management m ON s.ManagementNumber = m.ManagementNumber
    WHERE 
        w.RealEndDate > w.PlannedEndDate
    GROUP BY 
        wt.WorkTypeID, wt.WorkTypeName, m.ManagementNumber, s.SectionName
    ORDER BY 
        m.ManagementNumber, s.SectionName, COUNT(*) DESC;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_work_type_late_management` (`management_id` INT)   BEGIN
    SELECT 
        wt.WorkTypeID,
        wt.WorkTypeName,
        s.SectionName,
        COUNT(*) AS LateCount,
        AVG(DATEDIFF(w.RealEndDate, w.PlannedEndDate)) AS AvgDelayDays
    FROM 
        work w
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    JOIN management m ON s.ManagementNumber = m.ManagementNumber
    WHERE 
        m.ManagementNumber = management_id
        AND w.RealEndDate > w.PlannedEndDate
    GROUP BY 
        wt.WorkTypeID, wt.WorkTypeName, s.SectionName
    ORDER BY 
        s.SectionName, COUNT(*) DESC;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_work_type_late_section` (`section_id` INT)   BEGIN
    SELECT 
        wt.WorkTypeID,
        wt.WorkTypeName,
        COUNT(*) AS LateCount,
        AVG(DATEDIFF(w.RealEndDate, w.PlannedEndDate)) AS AvgDelayDays,
        MAX(DATEDIFF(w.RealEndDate, w.PlannedEndDate)) AS MaxDelayDays
    FROM 
        work w
    JOIN work_type wt ON w.WorkTypeID = wt.WorkTypeID
    JOIN object o ON w.ObjectNameID = o.ObjectNameID
    JOIN section s ON o.SectionNameID = s.SectionNameID
    WHERE 
        s.SectionNameID = section_id
        AND w.RealEndDate > w.PlannedEndDate
    GROUP BY 
        wt.WorkTypeID, wt.WorkTypeName
    ORDER BY 
        COUNT(*) DESC;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `brigade`
--

CREATE TABLE `brigade` (
  `BrigadeID` int NOT NULL,
  `Foreman` int DEFAULT NULL,
  `BrigadeName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `brigade`
--

INSERT INTO `brigade` (`BrigadeID`, `Foreman`, `BrigadeName`) VALUES
(1, 9, 'Каменьщики'),
(2, 13, 'Маляры'),
(3, 16, 'Отделочники'),
(4, 19, 'Бетонщики'),
(5, 22, 'Электрики'),
(6, 25, 'Слесари'),
(7, 34, 'Водопроводчики'),
(8, 39, 'Бетонщики'),
(9, 42, 'Геодезисты'),
(10, 44, 'Сварщики');

-- --------------------------------------------------------

--
-- Структура таблицы `brigade_employee`
--

CREATE TABLE `brigade_employee` (
  `EmployeeCode` int NOT NULL,
  `BrigadeID` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `brigade_employee`
--

INSERT INTO `brigade_employee` (`EmployeeCode`, `BrigadeID`) VALUES
(10, 1),
(11, 2),
(12, 2),
(14, 3),
(15, 3),
(17, 4),
(18, 4),
(20, 5),
(21, 5),
(23, 6),
(24, 6),
(30, 7),
(32, 7),
(33, 7),
(40, 9),
(41, 9),
(43, 9),
(45, 10);

-- --------------------------------------------------------

--
-- Структура таблицы `characteristic_gr`
--

CREATE TABLE `characteristic_gr` (
  `CharacteristicGrID` int NOT NULL,
  `CharacteristicGrName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `characteristic_gr`
--

INSERT INTO `characteristic_gr` (`CharacteristicGrID`, `CharacteristicGrName`) VALUES
(1, 'Минимальный разряд'),
(2, 'Требуемое образование'),
(3, 'Опыт работы (лет)'),
(4, 'Допуск к работе с техникой'),
(5, 'Категория водительских прав');

-- --------------------------------------------------------

--
-- Структура таблицы `characteristic_ob`
--

CREATE TABLE `characteristic_ob` (
  `CharacteristicObNameID` int NOT NULL,
  `CharacteristicObName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `characteristic_ob`
--

INSERT INTO `characteristic_ob` (`CharacteristicObNameID`, `CharacteristicObName`) VALUES
(1, 'Площадь'),
(2, 'Этажность'),
(3, 'Вместимость'),
(4, 'Протяженность'),
(5, 'Количество полос'),
(6, 'Количество этажей подземной части'),
(7, 'Количество торговых площадей');

-- --------------------------------------------------------

--
-- Структура таблицы `contract`
--

CREATE TABLE `contract` (
  `ContractNumber` int NOT NULL,
  `Price` decimal(12,2) NOT NULL,
  `CustomerName` varchar(45) NOT NULL
) ;

--
-- Дамп данных таблицы `contract`
--

INSERT INTO `contract` (`ContractNumber`, `Price`, `CustomerName`) VALUES
(1, 500000.00, 'Bitls Rok Company'),
(2, 12000000.00, 'ООО ИнГрупп'),
(3, 20000000.00, 'Министертсво здравоохранения Пермского края'),
(4, 9000000.00, 'Администрация города Краснакамска');

-- --------------------------------------------------------

--
-- Структура таблицы `employee`
--

CREATE TABLE `employee` (
  `EmployeeCode` int NOT NULL,
  `GroupNameID` int DEFAULT NULL,
  `FirstName` varchar(45) NOT NULL,
  `LastName` varchar(45) DEFAULT NULL,
  `HireDate` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `employee`
--

INSERT INTO `employee` (`EmployeeCode`, `GroupNameID`, `FirstName`, `LastName`, `HireDate`) VALUES
(4, 1, 'Петр', 'Иванович', '2005-07-20'),
(5, 1, 'Артем', 'Андреевич', '2003-05-20'),
(6, 1, 'Илья', 'Иванович', '2023-07-20'),
(7, 5, 'Владимир', 'Красно-Солнышко', '2013-09-20'),
(8, 5, 'Боб', 'Марли', '2001-04-20'),
(9, 3, 'Джон', 'Буш', '2007-07-07'),
(10, 2, 'Джон', 'Уик', '2008-08-08'),
(11, 4, 'Юрий', 'Никулин', '2002-11-11'),
(12, 4, 'Георгий', 'Вицин', '2002-11-11'),
(13, 3, 'Евгений', 'Моргунов', '2002-11-11'),
(14, 9, 'Андрей', 'Капица', '2002-09-13'),
(15, 9, 'Иван', 'Иванович', '2002-09-23'),
(16, 3, 'Виктор', 'Карелин', '2001-05-12'),
(17, 8, 'Илья', 'Калинин', '2003-05-20'),
(18, 8, 'Максим', 'Колесников', '2007-07-07'),
(19, 3, 'Георгий', 'Гаина', '2001-04-08'),
(20, 11, 'Денис', 'Ямаев', '2007-07-07'),
(21, 11, 'Константин', 'Рыбьяков', '2003-05-20'),
(22, 3, 'Дмитрий', 'Химера', '2002-09-13'),
(23, 13, 'Артем', 'Першин', '2002-11-11'),
(24, 13, 'Артем', 'Паршин', '2001-04-08'),
(25, 3, 'Кондратий', 'Козлюк', '2005-07-20'),
(26, 5, 'Максим', 'Елохов', '2007-07-07'),
(27, 5, 'Егор', 'Ившин', '2001-04-08'),
(28, 6, 'Мюрат', 'Бонапарт', '2005-07-20'),
(29, 6, 'Даву', 'Артаньян', '2008-08-08'),
(30, 18, 'Максим', 'Камалетдинов', '2025-05-13'),
(32, 18, 'Максим', 'Филатов', '2007-07-07'),
(33, 18, 'Максим', 'Горький', '2002-11-11'),
(34, 3, 'Карл', 'Маркс', '2002-11-11'),
(35, 5, 'Артур', 'Кукулькан', '2013-09-20'),
(36, 5, 'Арнольд', 'Шфарценегер', '2001-04-20'),
(37, 8, 'Василий', 'Ходырев', '2001-05-12'),
(38, 8, 'Анатолий', 'Чепурин', '2002-11-11'),
(39, 3, 'Сергей', 'Важенин', '2005-07-20'),
(40, 20, 'Андрей', 'Ширинкин', '2005-07-20'),
(41, 12, 'Антон', 'Беломоров', '2013-09-20'),
(42, 3, 'Григорий', 'Шестаков', '2001-05-12'),
(43, 19, 'Валентин', 'Петушков', '2008-08-08'),
(44, 3, 'Егор', 'Юдин', '2001-05-13'),
(45, 10, 'Олег', 'Корепав', '2001-04-20'),
(50, 1, 'Евгений', 'Шаламов', '2005-07-20'),
(51, 5, 'Альберт', 'Эйнштейн', '2001-05-12'),
(52, 6, 'Амодей', 'Моцарт', '2001-05-29'),
(53, 7, 'Себастьян', 'Бах', '2001-05-18'),
(61, NULL, 'НовыйСотрудникИмя2', 'НовыйСотрудникФамилия2', '2025-05-14'),
(63, 5, 'Илья', 'Норов', '2025-05-14');

-- --------------------------------------------------------

--
-- Структура таблицы `estimate`
--

CREATE TABLE `estimate` (
  `WorkNumber` int NOT NULL,
  `MaterialID` int NOT NULL,
  `Cost` decimal(10,2) DEFAULT NULL,
  `PlannedQuantity` decimal(9,3) NOT NULL,
  `RealQuantity` decimal(10,3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `estimate`
--

INSERT INTO `estimate` (`WorkNumber`, `MaterialID`, `Cost`, `PlannedQuantity`, `RealQuantity`) VALUES
(1, 2, 4500.00, 7.000, 7.500),
(1, 3, 7.00, 200.000, 250.300),
(2, 4, 1100.00, 300.000, 330.000),
(4, 1, 5000.00, 10.000, 10.000),
(5, 5, 100.00, 50.000, 60.000),
(7, 1, 5000.00, 30.000, 32.000),
(8, 2, 4500.00, 27.000, 28.000),
(8, 3, 10.00, 600.000, 600.600),
(9, 4, 1200.00, 400.000, 400.000),
(10, 5, 110.00, 150.000, 150.000),
(12, 1, 5000.00, 28.000, 30.000),
(13, 2, 4600.00, 25.000, 30.000),
(13, 3, 8.00, 550.000, 600.000),
(14, 4, 1100.00, 390.000, 400.000),
(15, 5, 120.00, 120.000, 150.000),
(17, 6, 8.00, 1200.000, 8.000),
(18, 7, 4.00, 25000.000, 4.000),
(19, 8, 200.00, 1200.000, 220.000);

--
-- Триггеры `estimate`
--
DELIMITER $$
CREATE TRIGGER `trg_estimate_quantity_check` BEFORE INSERT ON `estimate` FOR EACH ROW BEGIN
    IF NEW.RealQuantity < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Фактическое количество материалов не может быть отрицательным';
    END IF;
    
    IF NEW.Cost <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Стоимость материала должна быть положительной';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `group`
--

CREATE TABLE `group` (
  `GroupNameID` int NOT NULL,
  `GroupName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `group`
--

INSERT INTO `group` (`GroupNameID`, `GroupName`) VALUES
(8, 'Бетонщик'),
(3, 'Бригадир'),
(19, 'Геодезист'),
(5, 'Инженер'),
(2, 'Каменьщик'),
(4, 'Маляр'),
(16, 'Мастер'),
(15, 'Начальник управления'),
(14, 'Начальник участка'),
(9, 'Отделочник'),
(1, 'Проектировщик'),
(17, 'Прораб'),
(20, 'Рабочий-земляной'),
(18, 'Сантехник'),
(10, 'Сварщик'),
(13, 'Слесарь'),
(7, 'Техник'),
(6, 'Технолог'),
(12, 'Шофер'),
(11, 'Электрик');

-- --------------------------------------------------------

--
-- Структура таблицы `group_characteristic`
--

CREATE TABLE `group_characteristic` (
  `GroupNameID` int NOT NULL,
  `CharacteristicGrID` int NOT NULL,
  `ValueGrCh` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `group_characteristic`
--

INSERT INTO `group_characteristic` (`GroupNameID`, `CharacteristicGrID`, `ValueGrCh`) VALUES
(1, 1, '5'),
(1, 2, 'Высшее'),
(1, 3, '3'),
(2, 1, '4'),
(2, 2, 'Среднее'),
(2, 3, '2'),
(3, 1, '6'),
(3, 2, 'Среднее'),
(3, 3, '5'),
(4, 1, '3'),
(4, 2, 'Среднее'),
(4, 3, '1'),
(5, 1, '6'),
(5, 2, 'Высшее'),
(5, 3, '3'),
(6, 1, '5'),
(6, 2, 'Высшее'),
(6, 3, '2'),
(7, 1, '4'),
(7, 2, 'Среднее'),
(7, 3, '1'),
(8, 1, '4'),
(8, 2, 'Среднее'),
(8, 3, '2'),
(9, 1, '4'),
(9, 2, 'Среднее'),
(9, 3, '2'),
(10, 1, '5'),
(10, 2, 'Среднее'),
(10, 3, '3'),
(10, 4, 'Да'),
(11, 1, '5'),
(11, 2, 'Среднее'),
(11, 3, '3'),
(12, 1, '4'),
(12, 2, 'Среднее'),
(12, 3, '2'),
(12, 5, 'C'),
(13, 1, '4'),
(13, 2, 'Среднее'),
(13, 3, '2'),
(14, 1, '7'),
(14, 2, 'Высшее'),
(14, 3, '5'),
(15, 1, '8'),
(15, 2, 'Высшее'),
(15, 3, '7'),
(16, 1, '6'),
(16, 2, 'Среднее'),
(16, 3, '4'),
(17, 1, '7'),
(17, 2, 'Высшее'),
(17, 3, '5'),
(18, 1, '4'),
(18, 2, 'Среднее'),
(18, 3, '2'),
(19, 1, '5'),
(19, 2, 'Высшее'),
(19, 3, '3'),
(20, 1, '3'),
(20, 2, 'Нет'),
(20, 3, '0.5');

-- --------------------------------------------------------

--
-- Структура таблицы `machine`
--

CREATE TABLE `machine` (
  `SerialNumber` int NOT NULL,
  `ManagementNumber` int DEFAULT NULL,
  `MachineTypeID` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `machine`
--

INSERT INTO `machine` (`SerialNumber`, `ManagementNumber`, `MachineTypeID`) VALUES
(101, 1, 1),
(202, 1, 2),
(303, 1, 3);

-- --------------------------------------------------------

--
-- Структура таблицы `machine_type`
--

CREATE TABLE `machine_type` (
  `MachineTypeID` int NOT NULL,
  `MachineType` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `machine_type`
--

INSERT INTO `machine_type` (`MachineTypeID`, `MachineType`) VALUES
(1, 'Трактор многофункциональный'),
(2, 'Бетоносмеситель'),
(3, 'Подъемный кран'),
(4, 'Экскаватор'),
(5, 'Бульдозер');

-- --------------------------------------------------------

--
-- Структура таблицы `management`
--

CREATE TABLE `management` (
  `ManagementNumber` int NOT NULL,
  `Director` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `management`
--

INSERT INTO `management` (`ManagementNumber`, `Director`) VALUES
(1, 7),
(2, 35);

--
-- Триггеры `management`
--
DELIMITER $$
CREATE TRIGGER `before_insert_management` BEFORE INSERT ON `management` FOR EACH ROW BEGIN

END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_insert_management_check` BEFORE INSERT ON `management` FOR EACH ROW BEGIN
    DECLARE is_manager INT;
    
    -- Проверяем, является ли новый директор менеджером какого-либо участка
    SELECT COUNT(*) INTO is_manager 
    FROM section 
    WHERE Manager = NEW.Director;
    
    IF is_manager > 0 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Нельзя назначить менеджера участка директором управления';
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_update_management` BEFORE UPDATE ON `management` FOR EACH ROW BEGIN

END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_update_management_check` BEFORE UPDATE ON `management` FOR EACH ROW BEGIN
    DECLARE is_manager INT;
    
    -- Проверяем, является ли новый директор менеджером какого-либо участка
    SELECT COUNT(*) INTO is_manager 
    FROM section 
    WHERE Manager = NEW.Director;
    
    IF is_manager > 0 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Нельзя назначить менеджера участка директором управления';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `material`
--

CREATE TABLE `material` (
  `MaterialID` int NOT NULL,
  `MeasurementUnits` varchar(35) DEFAULT NULL,
  `MaterialName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `material`
--

INSERT INTO `material` (`MaterialID`, `MeasurementUnits`, `MaterialName`) VALUES
(1, 'Кубический метр', 'Бетон'),
(2, 'Кубический метр', 'Кирпич'),
(3, 'Килограмм', 'Цемент'),
(4, 'Килограмм', 'Штукатурка'),
(5, 'Метр', 'Труба водопроводная пластиковая'),
(6, 'шт.', 'Железобетонные сваи'),
(7, 'шт.', 'Металлические балки'),
(8, 'Квадратный метр', 'Асфальтобетон');

-- --------------------------------------------------------

--
-- Структура таблицы `object`
--

CREATE TABLE `object` (
  `ObjectNameID` int NOT NULL,
  `Supervisor` int DEFAULT NULL,
  `SectionNameID` int DEFAULT NULL,
  `ContractNumber` int DEFAULT NULL,
  `ObjectTypeID` int DEFAULT NULL,
  `ObjectName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `object`
--

INSERT INTO `object` (`ObjectNameID`, `Supervisor`, `SectionNameID`, `ContractNumber`, `ObjectTypeID`, `ObjectName`) VALUES
(1, 8, 1, 1, 4, 'МАОУ СОШ №13'),
(2, 26, 1, 2, 2, 'ЖК Самолет'),
(3, 27, 2, 3, 3, 'ГКИБ №10'),
(4, 8, 3, 4, 5, 'Мулянский мост');

-- --------------------------------------------------------

--
-- Структура таблицы `object_characteristic`
--

CREATE TABLE `object_characteristic` (
  `ObjectNameID` int NOT NULL,
  `CharacteristicObName` int NOT NULL,
  `ValueObCh` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `object_characteristic`
--

INSERT INTO `object_characteristic` (`ObjectNameID`, `CharacteristicObName`, `ValueObCh`) VALUES
(1, 1, '5000'),
(1, 2, '3'),
(1, 3, '800'),
(2, 1, '15000'),
(2, 2, '9'),
(2, 6, '1'),
(3, 1, '20000'),
(3, 2, '5'),
(3, 3, '500'),
(4, 4, '150'),
(4, 5, '2');

-- --------------------------------------------------------

--
-- Структура таблицы `object_type`
--

CREATE TABLE `object_type` (
  `ObjectTypeID` int NOT NULL,
  `ObjectType` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `object_type`
--

INSERT INTO `object_type` (`ObjectTypeID`, `ObjectType`) VALUES
(1, 'Сцена'),
(2, 'Жилой дом'),
(3, 'Больница'),
(4, 'Школа'),
(5, 'Мост'),
(6, 'Дорога'),
(7, 'Торговый центр');

-- --------------------------------------------------------

--
-- Структура таблицы `section`
--

CREATE TABLE `section` (
  `SectionNameID` int NOT NULL,
  `Manager` int NOT NULL,
  `ManagementNumber` int DEFAULT NULL,
  `SectionName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `section`
--

INSERT INTO `section` (`SectionNameID`, `Manager`, `ManagementNumber`, `SectionName`) VALUES
(1, 6, 1, 'Свердловский'),
(2, 29, 1, 'Березниковский'),
(3, 36, 2, 'Краснакамский');

--
-- Триггеры `section`
--
DELIMITER $$
CREATE TRIGGER `before_insert_section` BEFORE INSERT ON `section` FOR EACH ROW BEGIN

END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_insert_section_check` BEFORE INSERT ON `section` FOR EACH ROW BEGIN
    DECLARE is_director INT;
    
    -- Проверяем, является ли новый менеджер директором какого-либо управления
    SELECT COUNT(*) INTO is_director 
    FROM management 
    WHERE Director = NEW.Manager;
    
    IF is_director > 0 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Нельзя назначить директора управления менеджером участка';
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_update_section` BEFORE UPDATE ON `section` FOR EACH ROW BEGIN

END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_update_section_check` BEFORE UPDATE ON `section` FOR EACH ROW BEGIN
    DECLARE is_director INT;
    
    -- Проверяем, является ли новый менеджер директором какого-либо управления
    SELECT COUNT(*) INTO is_director 
    FROM management 
    WHERE Director = NEW.Manager;
    
    IF is_director > 0 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Нельзя назначить директора управления менеджером участка';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `section_employee`
--

CREATE TABLE `section_employee` (
  `SectionNameID` int NOT NULL,
  `EmployeeCode` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `section_employee`
--

INSERT INTO `section_employee` (`SectionNameID`, `EmployeeCode`) VALUES
(1, 4),
(1, 5),
(2, 50),
(2, 51),
(3, 52),
(3, 53);

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `v_all_managers`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `v_all_managers` (
`Дата приема на работу` date
,`Должность` varchar(45)
,`Номер/ID` int
,`Тип подразделения` varchar(10)
,`ФИО руководителя` varchar(91)
);

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `v_brigade_foremen`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `v_brigade_foremen` (
`ID бригады` int
,`Должность` varchar(45)
,`Код бригадира` int
,`Количество членов бригады` bigint
,`Название бригады` varchar(45)
,`ФИО бригадира` varchar(91)
);

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `v_management_directors`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `v_management_directors` (
`Дата приема на работу` date
,`Должность` varchar(45)
,`Код сотрудника` int
,`Номер управления` int
,`ФИО директора` varchar(91)
);

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `v_management_structure`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `v_management_structure` (
`ID участка` int
,`Директор управления` varchar(91)
,`Количество объектов` bigint
,`Количество сотрудников` bigint
,`Название участка` varchar(45)
,`Начальник участка` varchar(91)
,`Номер управления` int
);

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `v_section_managers`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `v_section_managers` (
`ID участка` int
,`Дата приема на работу` date
,`Должность` varchar(45)
,`Код сотрудника` int
,`Название участка` varchar(45)
,`Номер управления` int
,`ФИО начальника` varchar(91)
);

-- --------------------------------------------------------

--
-- Структура таблицы `work`
--

CREATE TABLE `work` (
  `WorkNumber` int NOT NULL,
  `ObjectNameID` int NOT NULL,
  `SectionNameID` int NOT NULL,
  `WorkTypeID` int NOT NULL,
  `BrigadeID` int DEFAULT NULL,
  `PlannedStartDate` date NOT NULL,
  `PlannedEndDate` date NOT NULL,
  `RealStartDate` date DEFAULT NULL,
  `RealEndDate` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `work`
--

INSERT INTO `work` (`WorkNumber`, `ObjectNameID`, `SectionNameID`, `WorkTypeID`, `BrigadeID`, `PlannedStartDate`, `PlannedEndDate`, `RealStartDate`, `RealEndDate`) VALUES
(1, 1, 1, 1, 1, '2001-01-20', '2001-12-20', '2001-01-21', '2001-12-23'),
(2, 1, 1, 2, 2, '2001-12-20', '2002-01-20', '2001-12-23', '2002-01-30'),
(3, 1, 1, 3, 9, '2001-01-01', '2001-01-08', '2001-01-01', '2001-01-08'),
(4, 1, 1, 4, 8, '2001-01-09', '2001-01-16', '2001-01-09', '2001-01-21'),
(5, 1, 1, 5, 7, '2002-01-20', '2002-02-10', '2002-01-30', '2002-02-10'),
(6, 2, 1, 3, 9, '2002-02-15', '2002-02-20', '2002-02-15', '2002-02-20'),
(7, 2, 1, 4, 8, '2002-02-20', '2002-02-25', '2002-02-20', '2002-02-26'),
(8, 2, 1, 1, 1, '2002-02-25', '2002-03-10', '2002-02-26', '2002-03-15'),
(9, 2, 1, 2, 2, '2002-03-10', '2002-03-25', '2002-03-15', '2002-03-25'),
(10, 2, 1, 5, 7, '2002-03-25', '2002-03-30', '2002-03-25', '2002-03-30'),
(11, 3, 2, 3, 9, '2002-04-04', '2002-04-07', '2002-03-30', '2002-04-07'),
(12, 3, 2, 4, 8, '2002-04-07', '2002-04-10', '2002-04-07', '2002-04-13'),
(13, 3, 2, 1, 1, '2002-04-10', '2002-04-17', '2002-04-13', '2002-04-17'),
(14, 3, 2, 2, 2, '2002-04-17', '2002-04-23', '2002-04-17', '2002-04-24'),
(15, 3, 2, 5, 7, '2002-04-23', '2002-04-27', '2002-04-24', '2002-04-27'),
(16, 4, 3, 3, 9, '2002-04-27', '2002-04-30', '2002-04-27', '2002-04-30'),
(17, 4, 3, 6, 6, '2002-04-30', '2002-05-05', '2002-04-30', '2002-05-05'),
(18, 4, 3, 7, 10, '2002-05-05', '2002-05-15', '2002-05-05', '2002-05-15'),
(19, 4, 3, 8, 9, '2002-05-25', '2002-05-30', '2002-05-25', '2002-05-31');

--
-- Триггеры `work`
--
DELIMITER $$
CREATE TRIGGER `trg_work_dates_check` BEFORE INSERT ON `work` FOR EACH ROW BEGIN
    IF NEW.PlannedEndDate < NEW.PlannedStartDate THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Дата окончания работы не может быть раньше даты начала';
    END IF;
    
    IF NEW.RealEndDate IS NOT NULL AND NEW.RealStartDate IS NOT NULL THEN
        IF NEW.RealEndDate < NEW.RealStartDate THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Фактическая дата окончания не может быть раньше даты начала';
        END IF;
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `work_machine`
--

CREATE TABLE `work_machine` (
  `WorkNumber` int NOT NULL,
  `SerialNumber` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `work_machine`
--

INSERT INTO `work_machine` (`WorkNumber`, `SerialNumber`) VALUES
(3, 101),
(6, 101),
(11, 101),
(16, 101),
(4, 202),
(7, 202),
(12, 202);

-- --------------------------------------------------------

--
-- Структура таблицы `work_type`
--

CREATE TABLE `work_type` (
  `WorkTypeID` int NOT NULL,
  `WorkTypeName` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `work_type`
--

INSERT INTO `work_type` (`WorkTypeID`, `WorkTypeName`) VALUES
(1, 'Возведение стен'),
(2, 'Штукатурка стен'),
(3, 'Выравнивание грунта'),
(4, 'Заливка фундамента'),
(5, 'Прокладка водоснабжения '),
(6, 'Установка мостовых опор'),
(7, 'Сборка перекрытий'),
(8, 'Укладка асфальта');

-- --------------------------------------------------------

--
-- Структура для представления `v_all_managers`
--
DROP TABLE IF EXISTS `v_all_managers`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_all_managers`  AS SELECT 'Управление' AS `Тип подразделения`, `m`.`ManagementNumber` AS `Номер/ID`, concat(`e`.`LastName`,' ',`e`.`FirstName`) AS `ФИО руководителя`, `e`.`HireDate` AS `Дата приема на работу`, `g`.`GroupName` AS `Должность` FROM ((`management` `m` join `employee` `e` on((`m`.`Director` = `e`.`EmployeeCode`))) join `group` `g` on((`e`.`GroupNameID` = `g`.`GroupNameID`))) WHERE (`g`.`GroupName` = 'Начальник управления')union all select 'Участок' AS `Тип подразделения`,`s`.`SectionNameID` AS `Номер/ID`,concat(`e`.`LastName`,' ',`e`.`FirstName`) AS `ФИО руководителя`,`e`.`HireDate` AS `Дата приема на работу`,`g`.`GroupName` AS `Должность` from ((`section` `s` join `employee` `e` on((`s`.`Manager` = `e`.`EmployeeCode`))) join `group` `g` on((`e`.`GroupNameID` = `g`.`GroupNameID`))) where (`g`.`GroupName` = 'Начальник участка') order by `Тип подразделения`,`Номер/ID`  ;

-- --------------------------------------------------------

--
-- Структура для представления `v_brigade_foremen`
--
DROP TABLE IF EXISTS `v_brigade_foremen`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_brigade_foremen`  AS SELECT `b`.`BrigadeID` AS `ID бригады`, `b`.`BrigadeName` AS `Название бригады`, `e`.`EmployeeCode` AS `Код бригадира`, concat(`e`.`LastName`,' ',`e`.`FirstName`) AS `ФИО бригадира`, `g`.`GroupName` AS `Должность`, (select count(0) from `brigade_employee` `be` where (`be`.`BrigadeID` = `b`.`BrigadeID`)) AS `Количество членов бригады` FROM ((`brigade` `b` join `employee` `e` on((`b`.`Foreman` = `e`.`EmployeeCode`))) join `group` `g` on((`e`.`GroupNameID` = `g`.`GroupNameID`))) ORDER BY `b`.`BrigadeName` ASC ;

-- --------------------------------------------------------

--
-- Структура для представления `v_management_directors`
--
DROP TABLE IF EXISTS `v_management_directors`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_management_directors`  AS SELECT `m`.`ManagementNumber` AS `Номер управления`, `e`.`EmployeeCode` AS `Код сотрудника`, concat(`e`.`LastName`,' ',`e`.`FirstName`) AS `ФИО директора`, `e`.`HireDate` AS `Дата приема на работу`, `g`.`GroupName` AS `Должность` FROM ((`management` `m` join `employee` `e` on((`m`.`Director` = `e`.`EmployeeCode`))) join `group` `g` on((`e`.`GroupNameID` = `g`.`GroupNameID`))) WHERE (`g`.`GroupName` = 'Начальник управления') ORDER BY `m`.`ManagementNumber` ASC ;

-- --------------------------------------------------------

--
-- Структура для представления `v_management_structure`
--
DROP TABLE IF EXISTS `v_management_structure`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_management_structure`  AS SELECT `m`.`ManagementNumber` AS `Номер управления`, (select concat(`emp`.`LastName`,' ',`emp`.`FirstName`) from `employee` `emp` where (`emp`.`EmployeeCode` = `m`.`Director`)) AS `Директор управления`, `s`.`SectionNameID` AS `ID участка`, `s`.`SectionName` AS `Название участка`, concat(`e`.`LastName`,' ',`e`.`FirstName`) AS `Начальник участка`, (select count(0) from `object` `o` where (`o`.`SectionNameID` = `s`.`SectionNameID`)) AS `Количество объектов`, (select count(0) from `section_employee` `se` where (`se`.`SectionNameID` = `s`.`SectionNameID`)) AS `Количество сотрудников` FROM ((`management` `m` left join `section` `s` on((`m`.`ManagementNumber` = `s`.`ManagementNumber`))) left join `employee` `e` on((`s`.`Manager` = `e`.`EmployeeCode`))) ORDER BY `m`.`ManagementNumber` ASC, `s`.`SectionNameID` ASC ;

-- --------------------------------------------------------

--
-- Структура для представления `v_section_managers`
--
DROP TABLE IF EXISTS `v_section_managers`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_section_managers`  AS SELECT `s`.`SectionNameID` AS `ID участка`, `s`.`SectionName` AS `Название участка`, `e`.`EmployeeCode` AS `Код сотрудника`, concat(`e`.`LastName`,' ',`e`.`FirstName`) AS `ФИО начальника`, `e`.`HireDate` AS `Дата приема на работу`, `g`.`GroupName` AS `Должность`, `m`.`ManagementNumber` AS `Номер управления` FROM (((`section` `s` join `employee` `e` on((`s`.`Manager` = `e`.`EmployeeCode`))) join `group` `g` on((`e`.`GroupNameID` = `g`.`GroupNameID`))) join `management` `m` on((`s`.`ManagementNumber` = `m`.`ManagementNumber`))) WHERE (`g`.`GroupName` = 'Начальник участка') ORDER BY `s`.`SectionName` ASC ;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `brigade`
--
ALTER TABLE `brigade`
  ADD PRIMARY KEY (`BrigadeID`),
  ADD UNIQUE KEY `Foreman_UNIQUE` (`Foreman`),
  ADD KEY `Foreman_brigade_idx` (`Foreman`);

--
-- Индексы таблицы `brigade_employee`
--
ALTER TABLE `brigade_employee`
  ADD PRIMARY KEY (`EmployeeCode`,`BrigadeID`),
  ADD UNIQUE KEY `EmployeeCode_UNIQUE` (`EmployeeCode`),
  ADD KEY `BrigadeID_brigade_employee_idx` (`BrigadeID`);

--
-- Индексы таблицы `characteristic_gr`
--
ALTER TABLE `characteristic_gr`
  ADD PRIMARY KEY (`CharacteristicGrID`);

--
-- Индексы таблицы `characteristic_ob`
--
ALTER TABLE `characteristic_ob`
  ADD PRIMARY KEY (`CharacteristicObNameID`);

--
-- Индексы таблицы `contract`
--
ALTER TABLE `contract`
  ADD PRIMARY KEY (`ContractNumber`);

--
-- Индексы таблицы `employee`
--
ALTER TABLE `employee`
  ADD PRIMARY KEY (`EmployeeCode`),
  ADD KEY `GroupNameID_idx` (`GroupNameID`);

--
-- Индексы таблицы `estimate`
--
ALTER TABLE `estimate`
  ADD PRIMARY KEY (`WorkNumber`,`MaterialID`),
  ADD KEY `MaterialID_estimate_idx` (`MaterialID`);

--
-- Индексы таблицы `group`
--
ALTER TABLE `group`
  ADD PRIMARY KEY (`GroupNameID`),
  ADD UNIQUE KEY `GroupName_UNIQUE` (`GroupName`);

--
-- Индексы таблицы `group_characteristic`
--
ALTER TABLE `group_characteristic`
  ADD PRIMARY KEY (`GroupNameID`,`CharacteristicGrID`),
  ADD KEY `CharacteristicGrID_idx` (`CharacteristicGrID`);

--
-- Индексы таблицы `machine`
--
ALTER TABLE `machine`
  ADD PRIMARY KEY (`SerialNumber`),
  ADD KEY `ManagementNumber_machine_idx` (`ManagementNumber`),
  ADD KEY `MachineTypeID_machine_idx` (`MachineTypeID`);

--
-- Индексы таблицы `machine_type`
--
ALTER TABLE `machine_type`
  ADD PRIMARY KEY (`MachineTypeID`);

--
-- Индексы таблицы `management`
--
ALTER TABLE `management`
  ADD PRIMARY KEY (`ManagementNumber`),
  ADD UNIQUE KEY `Director_UNIQUE` (`Director`);

--
-- Индексы таблицы `material`
--
ALTER TABLE `material`
  ADD PRIMARY KEY (`MaterialID`);

--
-- Индексы таблицы `object`
--
ALTER TABLE `object`
  ADD PRIMARY KEY (`ObjectNameID`),
  ADD KEY `Supervisor_object_idx` (`Supervisor`),
  ADD KEY `ContractNumber_object_idx` (`ContractNumber`),
  ADD KEY `ObjectTypeID_object_idx` (`ObjectTypeID`),
  ADD KEY `SectionNameID_object_idx` (`SectionNameID`);

--
-- Индексы таблицы `object_characteristic`
--
ALTER TABLE `object_characteristic`
  ADD PRIMARY KEY (`ObjectNameID`,`CharacteristicObName`),
  ADD KEY `CharacteristicObName_object_characteristic_idx` (`CharacteristicObName`);

--
-- Индексы таблицы `object_type`
--
ALTER TABLE `object_type`
  ADD PRIMARY KEY (`ObjectTypeID`);

--
-- Индексы таблицы `section`
--
ALTER TABLE `section`
  ADD PRIMARY KEY (`SectionNameID`),
  ADD KEY `ManagerSection_idx` (`Manager`),
  ADD KEY `ManagementNumberSection_idx` (`ManagementNumber`);

--
-- Индексы таблицы `section_employee`
--
ALTER TABLE `section_employee`
  ADD PRIMARY KEY (`SectionNameID`,`EmployeeCode`),
  ADD UNIQUE KEY `EmployeeCode_UNIQUE` (`EmployeeCode`);

--
-- Индексы таблицы `work`
--
ALTER TABLE `work`
  ADD PRIMARY KEY (`WorkNumber`),
  ADD KEY `ObjectNameID_work_idx` (`ObjectNameID`),
  ADD KEY `SectionNameID _work_idx` (`SectionNameID`),
  ADD KEY `BrigadeID _work_idx` (`BrigadeID`),
  ADD KEY `WorkTypeID _work_idx` (`WorkTypeID`);

--
-- Индексы таблицы `work_machine`
--
ALTER TABLE `work_machine`
  ADD PRIMARY KEY (`WorkNumber`,`SerialNumber`),
  ADD KEY `SerialNumber_work_machine_idx` (`SerialNumber`);

--
-- Индексы таблицы `work_type`
--
ALTER TABLE `work_type`
  ADD PRIMARY KEY (`WorkTypeID`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `brigade`
--
ALTER TABLE `brigade`
  MODIFY `BrigadeID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT для таблицы `characteristic_gr`
--
ALTER TABLE `characteristic_gr`
  MODIFY `CharacteristicGrID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `characteristic_ob`
--
ALTER TABLE `characteristic_ob`
  MODIFY `CharacteristicObNameID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT для таблицы `contract`
--
ALTER TABLE `contract`
  MODIFY `ContractNumber` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `employee`
--
ALTER TABLE `employee`
  MODIFY `EmployeeCode` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=64;

--
-- AUTO_INCREMENT для таблицы `group`
--
ALTER TABLE `group`
  MODIFY `GroupNameID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT для таблицы `machine_type`
--
ALTER TABLE `machine_type`
  MODIFY `MachineTypeID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `management`
--
ALTER TABLE `management`
  MODIFY `ManagementNumber` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT для таблицы `material`
--
ALTER TABLE `material`
  MODIFY `MaterialID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT для таблицы `object`
--
ALTER TABLE `object`
  MODIFY `ObjectNameID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `object_type`
--
ALTER TABLE `object_type`
  MODIFY `ObjectTypeID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT для таблицы `section`
--
ALTER TABLE `section`
  MODIFY `SectionNameID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблицы `work`
--
ALTER TABLE `work`
  MODIFY `WorkNumber` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT для таблицы `work_type`
--
ALTER TABLE `work_type`
  MODIFY `WorkTypeID` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `brigade`
--
ALTER TABLE `brigade`
  ADD CONSTRAINT `Foreman_brigade` FOREIGN KEY (`Foreman`) REFERENCES `employee` (`EmployeeCode`) ON DELETE RESTRICT ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `brigade_employee`
--
ALTER TABLE `brigade_employee`
  ADD CONSTRAINT `BrigadeID_brigade_employee` FOREIGN KEY (`BrigadeID`) REFERENCES `brigade` (`BrigadeID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `EmployeeCode_brigade_employee` FOREIGN KEY (`EmployeeCode`) REFERENCES `employee` (`EmployeeCode`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `employee`
--
ALTER TABLE `employee`
  ADD CONSTRAINT `GroupNameIDEmployee` FOREIGN KEY (`GroupNameID`) REFERENCES `group` (`GroupNameID`) ON DELETE RESTRICT ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `estimate`
--
ALTER TABLE `estimate`
  ADD CONSTRAINT `MaterialID_estimate` FOREIGN KEY (`MaterialID`) REFERENCES `material` (`MaterialID`) ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `WorkNumber_estimate` FOREIGN KEY (`WorkNumber`) REFERENCES `work` (`WorkNumber`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `group_characteristic`
--
ALTER TABLE `group_characteristic`
  ADD CONSTRAINT `CharacteristicGrID` FOREIGN KEY (`CharacteristicGrID`) REFERENCES `characteristic_gr` (`CharacteristicGrID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `GroupNameID` FOREIGN KEY (`GroupNameID`) REFERENCES `group` (`GroupNameID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `machine`
--
ALTER TABLE `machine`
  ADD CONSTRAINT `MachineTypeID_machine` FOREIGN KEY (`MachineTypeID`) REFERENCES `machine_type` (`MachineTypeID`),
  ADD CONSTRAINT `ManagementNumber_machine` FOREIGN KEY (`ManagementNumber`) REFERENCES `management` (`ManagementNumber`) ON DELETE RESTRICT ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `management`
--
ALTER TABLE `management`
  ADD CONSTRAINT `ManagementDirector` FOREIGN KEY (`Director`) REFERENCES `employee` (`EmployeeCode`) ON DELETE RESTRICT ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `object`
--
ALTER TABLE `object`
  ADD CONSTRAINT `ContractNumber_object` FOREIGN KEY (`ContractNumber`) REFERENCES `contract` (`ContractNumber`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `ObjectTypeID_object` FOREIGN KEY (`ObjectTypeID`) REFERENCES `object_type` (`ObjectTypeID`) ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `SectionNameID_object` FOREIGN KEY (`SectionNameID`) REFERENCES `section` (`SectionNameID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Supervisor_object` FOREIGN KEY (`Supervisor`) REFERENCES `employee` (`EmployeeCode`) ON DELETE RESTRICT ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `object_characteristic`
--
ALTER TABLE `object_characteristic`
  ADD CONSTRAINT `CharacteristicObName_object_characteristic` FOREIGN KEY (`CharacteristicObName`) REFERENCES `characteristic_ob` (`CharacteristicObNameID`) ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `ObjectNameID_object_characteristic` FOREIGN KEY (`ObjectNameID`) REFERENCES `object` (`ObjectNameID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `section`
--
ALTER TABLE `section`
  ADD CONSTRAINT `ManagerSection` FOREIGN KEY (`Manager`) REFERENCES `employee` (`EmployeeCode`) ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `section_employee`
--
ALTER TABLE `section_employee`
  ADD CONSTRAINT `EmployeeCode_section_employee` FOREIGN KEY (`EmployeeCode`) REFERENCES `employee` (`EmployeeCode`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `SectionNameID_section_employee` FOREIGN KEY (`SectionNameID`) REFERENCES `section` (`SectionNameID`) ON DELETE RESTRICT ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `work`
--
ALTER TABLE `work`
  ADD CONSTRAINT `BrigadeID_work` FOREIGN KEY (`BrigadeID`) REFERENCES `brigade` (`BrigadeID`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `ObjectNameID_work` FOREIGN KEY (`ObjectNameID`) REFERENCES `object` (`ObjectNameID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `SectionNameID_work` FOREIGN KEY (`SectionNameID`) REFERENCES `section` (`SectionNameID`),
  ADD CONSTRAINT `WorkTypeID_work` FOREIGN KEY (`WorkTypeID`) REFERENCES `work_type` (`WorkTypeID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `work_machine`
--
ALTER TABLE `work_machine`
  ADD CONSTRAINT `SerialNumber_work_machine` FOREIGN KEY (`SerialNumber`) REFERENCES `machine` (`SerialNumber`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `WorkNumber_work_machine` FOREIGN KEY (`WorkNumber`) REFERENCES `work` (`WorkNumber`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

﻿CREATE DATABASE QL_KHACHSAN
GO
USE QL_KHACHSAN
GO

CREATE TABLE KHACHHANG
(
	MAKH INT NOT NULL IDENTITY(1, 1), 
	TENKH NVARCHAR(50) NOT NULL, 
	NGSINH DATE,
	GIOITINH NVARCHAR(3) NOT NULL,
	SDT CHAR(10) NOT NULL, 
	MADD VARCHAR(12) NOT NULL, --MÃ ĐỊNH DANH: CCCD | THỊ THỰC
	QUOCTICH NVARCHAR(20),
	CONSTRAINT PK_KHACHHANG PRIMARY KEY(MAKH),
	CONSTRAINT CHK_GIOITINH_KHACHHANG CHECK (GIOITINH IN (N'Nam',N'Nữ')),
	CONSTRAINT CHK_SDT_KHACHHANG CHECK (LEN(SDT) = 10 AND SDT LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT UNI_MADD_KHACHHANG UNIQUE (MADD)
)

CREATE TABLE NHANVIEN
(
	MANV INT NOT NULL IDENTITY(1, 1), 
	TENNV NVARCHAR(50) NOT NULL, 
	NGSINH DATE, 
	SDT CHAR(10) NOT NULL,
	CCCD VARCHAR(12) NOT NULL, 
	DIACHI NVARCHAR(MAX) NOT NULL, 
	GIOITINH NVARCHAR(3) NOT NULL, 
	CHUCVU NVARCHAR(MAX) NOT NULL, 
	NGAYVAOLAM DATE NOT NULL,
	CONSTRAINT PK_NHANVIEN PRIMARY KEY (MANV),
	CONSTRAINT CHK_GIOITINH_NHANVIEN CHECK (GIOITINH IN (N'Nam',N'Nữ')),
	CONSTRAINT CHK_SDT_NHANVIEN CHECK (LEN(SDT) = 10 AND SDT LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT CHK_CCCD_NHANVIEN CHECK (LEN(CCCD) = 12 AND CCCD LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	CONSTRAINT CHK_NGAYVAOLAM CHECK (NGAYVAOLAM <= GETDATE()),
	CONSTRAINT UNI_CCCD_NHANVIEN UNIQUE (CCCD),
	CONSTRAINT UNI_SDT_NHANVIEN UNIQUE (SDT),
)

CREATE TABLE TAIKHOAN
(
	MANV INT NOT NULL,
	MATKHAU VARCHAR(MAX) NOT NULL,
	VAITRO NVARCHAR(MAX) NOT NULL,
	NGAYTAO DATETIME NOT NULL,
	NGAYDOIMK DATETIME NOT NULL,
	HOATDONG BIT NOT NULL,
	CONSTRAINT PK_TAIKHOAN PRIMARY KEY (MANV),
	CONSTRAINT FK_TAIKHOAN_NHANVIEN FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV),
	CONSTRAINT CHK_VAITRO CHECK (VAITRO IN (N'Quản trị viên', N'Tiếp tân'))
)

ALTER TABLE TAIKHOAN ADD
	CONSTRAINT DEF_NGAYTAO DEFAULT GETDATE() FOR NGAYTAO,
	CONSTRAINT DEF_NGAYDOIMK DEFAULT GETDATE() FOR NGAYDOIMK,
	CONSTRAINT DEF_VAITRO DEFAULT N'Tiếp tân' FOR VAITRO,
	CONSTRAINT DEF_HOATDONG DEFAULT 1 FOR HOATDONG

CREATE TABLE PHIEUDATPHONG
(
	MAPHIEUDATPHONG INT NOT NULL IDENTITY(1, 1),
	NGAYLAP DATETIME NOT NULL,
	NGAYDEN DATE NOT NULL, 
	NGAYTRADUKIEN DATE,
	MANV INT NOT NULL, 
	MAKH INT NOT NULL,
	CONSTRAINT PK_PHIEUDATPHONG PRIMARY KEY (MAPHIEUDATPHONG),
	CONSTRAINT FK_PHIEUDATPHONG_NHANVIEN FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV),
	CONSTRAINT FK_PHIEUDATPHONG_KHACHHANG FOREIGN KEY (MAKH) REFERENCES KHACHHANG(MAKH),
	CONSTRAINT CHK_NGAYDEN CHECK (NGAYDEN >= NGAYLAP)
)

ALTER TABLE PHIEUDATPHONG ADD 
	CONSTRAINT DEF_NGAYLAP DEFAULT GETDATE() FOR NGAYLAP

CREATE TABLE LOAIPHONG
(
	MALOAIPHONG INT NOT NULL IDENTITY(1, 1), 
	TENLOAIPHONG NVARCHAR(30) NOT NULL, 
	GIA MONEY NOT NULL, 
	SOLUONGNGUOITOIDA INT NOT NULL,
	CONSTRAINT PK_LOAIPHONG PRIMARY KEY (MALOAIPHONG),
	CONSTRAINT CHK_GIA CHECK (GIA > 0),
	CONSTRAINT CHK_SOLUONGNGUOITOIDA CHECK (SOLUONGNGUOITOIDA > 0)
)

CREATE TABLE PHONG
(
	MAPHONG INT NOT NULL IDENTITY(1, 1), 
	SOPHONG VARCHAR(10) NOT NULL UNIQUE, 
	TINHTRANG NVARCHAR(20) NOT NULL, 
	MALOAIPHONG INT NOT NULL,
	CONSTRAINT PK_PHONG PRIMARY KEY (MAPHONG),
	CONSTRAINT FK_PHONG_LOAIPHONG FOREIGN KEY (MALOAIPHONG) REFERENCES LOAIPHONG(MALOAIPHONG),
	CONSTRAINT CHK_TINHTRANG CHECK (TINHTRANG IN (N'Đã đặt', N'Phòng trống',N'Đã nhận'))
)

CREATE TABLE CHITIETPHIEUDAT
(
	MAPHIEUDATPHONG INT NOT NULL,
	MAPHONG INT NOT NULL, 
	NGAYTRAPHONG DATE, 
	CONSTRAINT PK_CHITIETPHIEUDAT PRIMARY KEY (MAPHIEUDATPHONG, MAPHONG),
	CONSTRAINT FK_CHITIETPHIEUDAT_PHIEUDATPHONG FOREIGN KEY (MAPHIEUDATPHONG) REFERENCES PHIEUDATPHONG(MAPHIEUDATPHONG),
	CONSTRAINT FK_CHITIETPHIEUDAT_PHONG FOREIGN KEY (MAPHONG) REFERENCES PHONG(MAPHONG)
)
CREATE TABLE PHIEUNHANPHONG
(
	MAPHIEUNHANPHONG INT NOT NULL IDENTITY(1, 1),
	MAPHIEUDATPHONG INT NOT NULL,
	NGAYNHAN DATE NOT NULL,
	MANV INT NOT NULL,
	CONSTRAINT PK_PHIEUNHANPHONG PRIMARY KEY(MAPHIEUNHANPHONG),
	CONSTRAINT FK_PHIEUNHANPHONG_PHIEUDATPHONG FOREIGN KEY(MAPHIEUDATPHONG) REFERENCES PHIEUDATPHONG(MAPHIEUDATPHONG),
	CONSTRAINT FK_PHIEUNHANPHONG_NHANVIEN FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV),
)
ALTER TABLE PHIEUNHANPHONG ADD 
	CONSTRAINT DEF_NGAYNHAN DEFAULT GETDATE() FOR NGAYNHAN
CREATE TABLE DOITRAPHONG
(
	MADOITRA INT NOT NULL IDENTITY(1, 1), 
	MAPHIEUDATPHONG INT NOT NULL,
	PHONGCU INT NOT NULL, 
	PHONGMOI INT NOT NULL, 
	NGAYDOI DATETIME NOT NULL,
	LYDO NVARCHAR(MAX),
	CONSTRAINT PK_DOITRAPHONG PRIMARY KEY(MADOITRA),
	CONSTRAINT FK_DOITRAPHONG_CHITIETPHIEUDAT FOREIGN KEY (MAPHIEUDATPHONG, PHONGCU) REFERENCES CHITIETPHIEUDAT(MAPHIEUDATPHONG, MAPHONG),
	CONSTRAINT FK_DOITRAPHONG_PHONGMOI FOREIGN KEY (PHONGMOI) REFERENCES PHONG(MAPHONG),
)

CREATE TABLE HOADON
(
	MAHD INT NOT NULL IDENTITY(1, 1), 
	NGAYLAP DATETIME NOT NULL, 
	TIENPHONG MONEY,
	TIENDICHVU MONEY,
	TONGTIEN MONEY, 
	MANV INT NOT NULL, 
	MAPHIEUDATPHONG INT NOT NULL,
	CONSTRAINT PK_HOADON PRIMARY KEY (MAHD),
	CONSTRAINT FK_HOADON_NHANVIEN FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV),
	CONSTRAINT FK_HOADON_PHIEUDATPHONG FOREIGN KEY (MAPHIEUDATPHONG) REFERENCES PHIEUDATPHONG(MAPHIEUDATPHONG)
)

CREATE TABLE PHIEUDICHVU
(
	MAPHIEUDV INT NOT NULL IDENTITY(1, 1), 
	NGAYLAP DATETIME NOT NULL, 
	TONGTIEN MONEY, 
	MAPHIEUDATPHONG INT NOT NULL,
	MANV INT NOT NULL,
	CONSTRAINT PK_PHIEUDICHVU PRIMARY KEY (MAPHIEUDV),
	CONSTRAINT FK_PHIEUDICHVU_PHIEUDATPHONG FOREIGN KEY (MAPHIEUDATPHONG) REFERENCES PHIEUDATPHONG(MAPHIEUDATPHONG),
	CONSTRAINT FK_PHIEUDICHVU_NHANVIEN FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV)
)

ALTER TABLE PHIEUDICHVU ADD 
	CONSTRAINT DEF_NGAYLAP_PHIEUDV DEFAULT GETDATE() FOR NGAYLAP

CREATE TABLE DICHVU
(
	MADV INT NOT NULL IDENTITY(1, 1), 
	TENDV NVARCHAR(30) NOT NULL, 
	DONGIA MONEY NOT NULL,
	CONSTRAINT PK_DICHVU PRIMARY KEY (MADV),
	CONSTRAINT CHK_DONGIADV CHECK (DONGIA > 0)
)

CREATE TABLE CHITIETPHIEUDV
(
	MAPHIEUDV INT NOT NULL,
	MADV INT NOT NULL, 
	SOLUONG INT NOT NULL,
	CONSTRAINT PK_CHITIETPHIEUDV PRIMARY KEY (MAPHIEUDV, MADV),
	CONSTRAINT FK_CHITIETPHIEUDV_PHIEUDICHVU FOREIGN KEY (MAPHIEUDV) REFERENCES PHIEUDICHVU(MAPHIEUDV),
	CONSTRAINT FK_CHITIETPHIEUDV_DICHVU FOREIGN KEY (MADV) REFERENCES DICHVU(MADV),
	CONSTRAINT CHK_SOLUONG CHECK (SOLUONG > 0)
)

CREATE TABLE THANHTOAN
(
	MATHANHTOAN INT NOT NULL IDENTITY(1, 1),
	MAHD INT NOT NULL,
	PTTHANHTOAN NVARCHAR(20) NOT NULL,
	TRANGTHAI NVARCHAR(20) NOT NULL,
	HANTHANHTOAN DATETIME,
	NGAYTHANHTOAN DATETIME,
	CONSTRAINT PK_THANHTOAN PRIMARY KEY (MATHANHTOAN),
	CONSTRAINT FK_THANHTOAN_HOADON FOREIGN KEY (MAHD) REFERENCES HOADON(MAHD),
	CONSTRAINT CHK_PTTHANHTOAN CHECK (PTTHANHTOAN IN (N'Tiền mặt', N'Chuyển khoản')),
	CONSTRAINT CHK_TRANGTHAI CHECK (TRANGTHAI IN (N'Đang đợi', N'Thành công', N'Thất bại')),
	CONSTRAINT CHK_NGAYTHANHTOAN CHECK (NGAYTHANHTOAN > GETDATE() AND NGAYTHANHTOAN < HANTHANHTOAN),
)

ALTER TABLE THANHTOAN ADD 
	CONSTRAINT DEF_HANTHANHTOAN DEFAULT DATEADD(DAY, 1, GETDATE()) FOR HANTHANHTOAN,
	CONSTRAINT DEF_TRANGTHAI DEFAULT N'Đang đợi' FOR TRANGTHAI

SET DATEFORMAT DMY
INSERT INTO KHACHHANG
VALUES  (N'Nguyễn Văn Tuấn','12/12/1995',N'Nam','0778523210','032120125212',N'Việt Nam'),
		(N'Nguyễn Thị Thơm','02/05/2005',N'Nam','0332123210','052320122030',N'Việt Nam'),
		(N'Trần Văn Hoàng','07/05/1995',N'Nam','0778201220','030230122012',N'Việt Nam')
INSERT INTO NHANVIEN
VALUES  (N'Phan Hồng Ngọc','03/08/1996','0247201230','020120301012', N'Tân Phú, Hồ Chí Minh',N'Nữ',N'Lễ tân','07/11/2020'),
		(N'Phan Ngọc Hữu','03/10/1997','0247202012','056200301012', N'Tân Phú, Hồ Chí Minh',N'Nam',N'Lễ tân','15/05/2023'),
		(N'Đặng Hữu Nghĩa','14/07/2000','0332001230','052201001012', N'Tân Phú, Hồ Chí Minh',N'Nam',N'Lễ tân','20/11/2020')
INSERT INTO LOAIPHONG
VALUES  (N'Phòng đơn',700000,2),
		(N'Phòng đôi',1200000,3),
		(N'Phòng Deluxe',2000000,2)
INSERT INTO PHONG
VALUES  ('P101',N'Phòng trống',1),
		('P102',N'Phòng trống',2),
		('P103',N'Phòng trống',3)
INSERT INTO PHIEUDATPHONG(NGAYDEN,NGAYTRADUKIEN,MANV,MAKH)
VALUES  ('12/11/2024','13/11/2024',1,1),
		('13/11/2024','15/11/2024',1,2),
		('12/11/2024','15/11/2024',1,3)
INSERT INTO CHITIETPHIEUDAT
VALUES  (1,1,NULL),
		(2,1,NULL),
		(3,2,NULL),
		(3,3,NULL)
INSERT INTO PHIEUNHANPHONG(MAPHIEUDATPHONG,MANV)
VALUES  (2,1)

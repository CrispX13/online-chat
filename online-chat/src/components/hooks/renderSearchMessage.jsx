// src/utils/renderSearchMessage.js
import React from "react";

// простая проверка URL
const urlRegex = /^(https?:\/\/[^\s]+)$/i;

export function renderSearchMessage(text) {
  if (!text) return null;

  const lines = text.split(/\r?\n/);

  const blocks = [];
  let currentBlock = [];

  const flushBlock = () => {
    if (currentBlock.length === 0) return;
    blocks.push(currentBlock);
    currentBlock = [];
  };

  for (const line of lines) {
    if (line.trim() === "") {
      flushBlock();
      continue;
    }
    currentBlock.push(line);
  }
  flushBlock();

  return blocks.map((blockLines, blockIndex) => {
    return (
      <div className="SearchMessageBlock" key={blockIndex}>
        {blockLines.map((line, lineIndex) => {
          const key = `${blockIndex}-${lineIndex}`;
          const trimmed = line.trim();

          // строка‑заголовок вида **...**
          if (trimmed.startsWith("**") && trimmed.endsWith("**")) {
            const inner = trimmed.slice(2, -2).trim();
            return (
              <div className="SearchMessage__title" key={key}>
                {inner}
              </div>
            );
          }

          // строка‑ссылка
          if (urlRegex.test(trimmed)) {
            return (
              <a
                key={key}
                className="SearchMessage__link"
                href={trimmed}
                target="_blank"
                rel="noopener noreferrer"
              >
                {trimmed}
              </a>
            );
          }

          // обычный текст (описание)
          return (
            <div className="SearchMessage__text" key={key}>
              {trimmed}
            </div>
          );
        })}
      </div>
    );
  });
}
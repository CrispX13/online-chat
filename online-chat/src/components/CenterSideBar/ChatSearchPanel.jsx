export default function ChatSearchPanel({
  visible,
  query,
  results,
  selectedIndexes,
  error,
  onToggle,
  onStop,
  onSendSelected
}) {
  if (!visible) return null;

  return (
    <div className="ChatSearchPanel">
      <div className="ChatSearchPanel__header">
        <div className="ChatSearchPanel__header-text">
          <div className="ChatSearchPanel__label">Web search</div>
          <div className="ChatSearchPanel__query">{query}</div>
        </div>

        <button
          type="button"
          className="ChatSearchPanel__stop"
          onClick={onStop}
        >
          Stop
        </button>
      </div>

      {error && <div className="ChatSearchPanel__error">{error}</div>}

      <div className="ChatSearchPanel__list">
        {results.map((item) => {
          const checked = selectedIndexes.includes(item.index);

          return (
            <label
              key={item.index}
              className={`ChatSearchPanel__item ${checked ? "selected" : ""}`}
            >
              <input
                type="checkbox"
                checked={checked}
                onChange={() => onToggle(item.index)}
              />

              <div className="ChatSearchPanel__content">
                <a
                  href={item.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="ChatSearchPanel__title"
                  onClick={(e) => e.stopPropagation()}
                >
                  {item.title}
                </a>

                <div className="ChatSearchPanel__url">{item.url}</div>

                {item.snippet && (
                  <div className="ChatSearchPanel__snippet">{item.snippet}</div>
                )}
              </div>
            </label>
          );
        })}
      </div>

      <div className="ChatSearchPanel__actions">
        <button
          type="button"
          className="ChatSearchPanel__button ChatSearchPanel__button--ghost"
          onClick={onStop}
        >
          Отменить
        </button>

        <button
          type="button"
          className="ChatSearchPanel__button ChatSearchPanel__button--primary"
          onClick={onSendSelected}
          disabled={selectedIndexes.length === 0}
        >
          Отправить в чат
        </button>
      </div>
    </div>
  );
}